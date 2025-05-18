using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.ExecutionBase.Models;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.IntegrationBase.Messaging;
using FlowOrchestrator.ProcessingBase.Messaging;

namespace FlowOrchestrator.ExecutionBase.Engine;

/// <summary>
/// Flow execution engine.
/// </summary>
public class FlowExecutionEngine
{
    private readonly IMongoRepository<FlowEntity> _flowRepository;
    private readonly IMongoRepository<SourceAssignmentEntity> _sourceAssignmentRepository;
    private readonly IMongoRepository<DestinationAssignmentEntity> _destinationAssignmentRepository;
    private readonly IMongoRepository<ProcessingChainEntity> _processingChainRepository;
    private readonly IMessageBus _messageBus;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowExecutionEngine"/> class.
    /// </summary>
    /// <param name="flowRepository">The flow repository.</param>
    /// <param name="sourceAssignmentRepository">The source assignment repository.</param>
    /// <param name="destinationAssignmentRepository">The destination assignment repository.</param>
    /// <param name="processingChainRepository">The processing chain repository.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public FlowExecutionEngine(
        IMongoRepository<FlowEntity> flowRepository,
        IMongoRepository<SourceAssignmentEntity> sourceAssignmentRepository,
        IMongoRepository<DestinationAssignmentEntity> destinationAssignmentRepository,
        IMongoRepository<ProcessingChainEntity> processingChainRepository,
        IMessageBus messageBus,
        ILogger logger,
        ITelemetryProvider telemetryProvider)
    {
        _flowRepository = flowRepository;
        _sourceAssignmentRepository = sourceAssignmentRepository;
        _destinationAssignmentRepository = destinationAssignmentRepository;
        _processingChainRepository = processingChainRepository;
        _messageBus = messageBus;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Executes a flow.
    /// </summary>
    /// <param name="flowId">The flow identifier.</param>
    /// <param name="sourceAssignmentId">The source assignment identifier.</param>
    /// <param name="destinationAssignmentIds">The destination assignment identifiers.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>The execution context.</returns>
    public async Task<FlowOrchestrator.ExecutionBase.Models.ExecutionContext> ExecuteFlowAsync(
        string flowId,
        string sourceAssignmentId,
        List<string> destinationAssignmentIds,
        string correlationId)
    {
        using var span = _telemetryProvider.CreateSpan("FlowExecutionEngine.ExecuteFlow");
        span.SetAttribute("flow.id", flowId);
        span.SetAttribute("source.assignment.id", sourceAssignmentId);
        span.SetAttribute("correlation.id", correlationId);

        var context = new FlowOrchestrator.ExecutionBase.Models.ExecutionContext
        {
            FlowId = flowId,
            SourceAssignmentId = sourceAssignmentId,
            DestinationAssignmentIds = destinationAssignmentIds,
            CorrelationId = correlationId
        };

        try
        {
            _logger.Info($"Starting flow execution for flow ID: {flowId}");
            await SendExecutionStatusAsync(context, ExecutionStatus.Initializing, "Initializing flow execution");

            // Load flow entity
            var flow = await _flowRepository.GetByIdAsync(flowId);
            if (flow == null)
            {
                throw new Exception($"Flow with ID {flowId} not found");
            }

            // Load source assignment
            var sourceAssignment = await _sourceAssignmentRepository.GetByIdAsync(sourceAssignmentId);
            if (sourceAssignment == null)
            {
                throw new Exception($"Source assignment with ID {sourceAssignmentId} not found");
            }

            // Load destination assignments
            var destinationAssignments = new List<DestinationAssignmentEntity>();
            foreach (var destinationAssignmentId in destinationAssignmentIds)
            {
                var destinationAssignment = await _destinationAssignmentRepository.GetByIdAsync(destinationAssignmentId);
                if (destinationAssignment == null)
                {
                    throw new Exception($"Destination assignment with ID {destinationAssignmentId} not found");
                }
                destinationAssignments.Add(destinationAssignment);
            }

            // Initialize branches
            foreach (var branchConfig in flow.BranchConfigurations)
            {
                var branch = new ExecutionBranch
                {
                    BranchId = branchConfig.Key,
                    BranchName = branchConfig.Value.BranchId,
                    ProcessingChainId = branchConfig.Value.ProcessingChainId,
                    DestinationAssignmentId = destinationAssignmentIds.FirstOrDefault() ?? string.Empty
                };
                context.Branches.Add(branch);
            }

            // Execute import
            await ExecuteImportAsync(context, sourceAssignment);

            // Execute processing and export for each branch
            foreach (var branch in context.Branches)
            {
                await ExecuteBranchAsync(context, branch, flow, destinationAssignments);
            }

            // Complete execution
            context.Status = ExecutionStatus.Completed;
            context.EndTime = DateTime.UtcNow;
            await SendExecutionStatusAsync(context, ExecutionStatus.Completed, "Flow execution completed");

            _logger.Info($"Flow execution completed for flow ID: {flowId}");
            span.SetStatus(SpanStatus.Ok);
            return context;
        }
        catch (Exception ex)
        {
            _logger.Error($"Flow execution failed for flow ID: {flowId}", ex);
            context.Status = ExecutionStatus.Failed;
            context.ErrorMessage = ex.Message;
            context.EndTime = DateTime.UtcNow;
            await SendExecutionStatusAsync(context, ExecutionStatus.Failed, $"Flow execution failed: {ex.Message}");

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return context;
        }
    }

    private async Task ExecuteImportAsync(FlowOrchestrator.ExecutionBase.Models.ExecutionContext context, SourceAssignmentEntity sourceAssignment)
    {
        using var span = _telemetryProvider.CreateSpan("FlowExecutionEngine.ExecuteImport");
        span.SetAttribute("flow.id", context.FlowId);
        span.SetAttribute("source.assignment.id", context.SourceAssignmentId);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Starting import for flow ID: {context.FlowId}");
            await SendExecutionStatusAsync(context, ExecutionStatus.Importing, "Importing data");

            var importStep = new ExecutionStep
            {
                StepName = "Import",
                StepType = "Import",
                Status = ExecutionStepStatus.Running
            };
            context.Steps.Add(importStep);

            // Create import parameters
            var importParameters = new ImportParameters
            {
                SourceId = sourceAssignment.SourceId,
                SourceAddress = string.Empty, // This would be retrieved from the source entity
                Configuration = sourceAssignment.Configuration
            };

            // Send import command
            var importCommand = new ImportCommand(
                sourceAssignment.ImporterId,
                importParameters,
                context.CorrelationId);

            await _messageBus.SendAsync(importCommand);

            // In a real implementation, we would wait for the import result
            // For now, we'll simulate a successful import
            var importResult = new ImportResult
            {
                Success = true,
                DataFormat = "JSON",
                Data = new { /* Sample data */ }
            };

            context.ImportResult = importResult;
            importStep.Status = ExecutionStepStatus.Completed;
            importStep.EndTime = DateTime.UtcNow;
            importStep.Result = importResult;

            _logger.Info($"Import completed for flow ID: {context.FlowId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Import failed for flow ID: {context.FlowId}", ex);
            var importStep = context.Steps.FirstOrDefault(s => s.StepName == "Import");
            if (importStep != null)
            {
                importStep.Status = ExecutionStepStatus.Failed;
                importStep.EndTime = DateTime.UtcNow;
                importStep.ErrorMessage = ex.Message;
            }

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private async Task ExecuteBranchAsync(
        FlowOrchestrator.ExecutionBase.Models.ExecutionContext context,
        ExecutionBranch branch,
        FlowEntity flow,
        List<DestinationAssignmentEntity> destinationAssignments)
    {
        using var span = _telemetryProvider.CreateSpan("FlowExecutionEngine.ExecuteBranch");
        span.SetAttribute("flow.id", context.FlowId);
        span.SetAttribute("branch.id", branch.BranchId);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Starting branch execution for branch ID: {branch.BranchId}");
            branch.Status = ExecutionBranchStatus.Running;

            // Execute processing
            await ExecuteProcessingAsync(context, branch, flow);

            // Execute export
            await ExecuteExportAsync(context, branch, destinationAssignments);

            branch.Status = ExecutionBranchStatus.Completed;
            branch.EndTime = DateTime.UtcNow;

            _logger.Info($"Branch execution completed for branch ID: {branch.BranchId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Branch execution failed for branch ID: {branch.BranchId}", ex);
            branch.Status = ExecutionBranchStatus.Failed;
            branch.EndTime = DateTime.UtcNow;
            branch.ErrorMessage = ex.Message;

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private async Task ExecuteProcessingAsync(FlowOrchestrator.ExecutionBase.Models.ExecutionContext context, ExecutionBranch branch, FlowEntity flow)
    {
        using var span = _telemetryProvider.CreateSpan("FlowExecutionEngine.ExecuteProcessing");
        span.SetAttribute("flow.id", context.FlowId);
        span.SetAttribute("branch.id", branch.BranchId);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Starting processing for branch ID: {branch.BranchId}");
            await SendExecutionStatusAsync(context, ExecutionStatus.Processing, $"Processing data for branch {branch.BranchId}");

            var processingStep = new ExecutionStep
            {
                StepName = "Processing",
                StepType = "Processing",
                Status = ExecutionStepStatus.Running
            };
            branch.Steps.Add(processingStep);

            // Load processing chain
            var processingChain = await _processingChainRepository.GetByIdAsync(branch.ProcessingChainId);
            if (processingChain == null)
            {
                throw new Exception($"Processing chain with ID {branch.ProcessingChainId} not found");
            }

            // In a real implementation, we would execute the processing chain
            // For now, we'll simulate a successful processing
            var processingResult = new ProcessingResult
            {
                Success = true,
                DataFormat = "JSON",
                Data = context.ImportResult?.Data
            };

            context.ProcessingResults[branch.BranchId] = processingResult;
            processingStep.Status = ExecutionStepStatus.Completed;
            processingStep.EndTime = DateTime.UtcNow;
            processingStep.Result = processingResult;

            _logger.Info($"Processing completed for branch ID: {branch.BranchId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Processing failed for branch ID: {branch.BranchId}", ex);
            var processingStep = branch.Steps.FirstOrDefault(s => s.StepName == "Processing");
            if (processingStep != null)
            {
                processingStep.Status = ExecutionStepStatus.Failed;
                processingStep.EndTime = DateTime.UtcNow;
                processingStep.ErrorMessage = ex.Message;
            }

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private async Task ExecuteExportAsync(
        FlowOrchestrator.ExecutionBase.Models.ExecutionContext context,
        ExecutionBranch branch,
        List<DestinationAssignmentEntity> destinationAssignments)
    {
        using var span = _telemetryProvider.CreateSpan("FlowExecutionEngine.ExecuteExport");
        span.SetAttribute("flow.id", context.FlowId);
        span.SetAttribute("branch.id", branch.BranchId);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Starting export for branch ID: {branch.BranchId}");
            await SendExecutionStatusAsync(context, ExecutionStatus.Exporting, $"Exporting data for branch {branch.BranchId}");

            var exportStep = new ExecutionStep
            {
                StepName = "Export",
                StepType = "Export",
                Status = ExecutionStepStatus.Running
            };
            branch.Steps.Add(exportStep);

            // Find the destination assignment for this branch
            var destinationAssignment = destinationAssignments.FirstOrDefault(da => da.Id == branch.DestinationAssignmentId);
            if (destinationAssignment == null)
            {
                throw new Exception($"Destination assignment with ID {branch.DestinationAssignmentId} not found");
            }

            // Create export parameters
            var exportParameters = new ExportParameters
            {
                DestinationId = destinationAssignment.DestinationId,
                DestinationAddress = string.Empty, // This would be retrieved from the destination entity
                Data = context.ProcessingResults[branch.BranchId].Data,
                DataFormat = context.ProcessingResults[branch.BranchId].DataFormat,
                Configuration = destinationAssignment.Configuration
            };

            // Send export command
            var exportCommand = new ExportCommand(
                destinationAssignment.ExporterId,
                exportParameters,
                context.CorrelationId);

            await _messageBus.SendAsync(exportCommand);

            // In a real implementation, we would wait for the export result
            // For now, we'll simulate a successful export
            var exportResult = new ExportResult
            {
                Success = true,
                DestinationId = destinationAssignment.DestinationId
            };

            context.ExportResults[branch.BranchId] = exportResult;
            exportStep.Status = ExecutionStepStatus.Completed;
            exportStep.EndTime = DateTime.UtcNow;
            exportStep.Result = exportResult;

            _logger.Info($"Export completed for branch ID: {branch.BranchId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Export failed for branch ID: {branch.BranchId}", ex);
            var exportStep = branch.Steps.FirstOrDefault(s => s.StepName == "Export");
            if (exportStep != null)
            {
                exportStep.Status = ExecutionStepStatus.Failed;
                exportStep.EndTime = DateTime.UtcNow;
                exportStep.ErrorMessage = ex.Message;
            }

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private async Task SendExecutionStatusAsync(FlowOrchestrator.ExecutionBase.Models.ExecutionContext context, ExecutionStatus status, string message)
    {
        context.Status = status;
        var statusMessage = new FlowExecutionStatusMessage(
            context.ExecutionId,
            context.FlowId,
            status.ToString(),
            message,
            context.CorrelationId);

        await _messageBus.PublishAsync(statusMessage);
    }
}

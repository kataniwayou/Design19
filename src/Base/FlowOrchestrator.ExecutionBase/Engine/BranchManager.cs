using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.ExecutionBase.Models;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;

namespace FlowOrchestrator.ExecutionBase.Engine;

/// <summary>
/// Branch manager.
/// </summary>
public class BranchManager
{
    private readonly IMongoRepository<ProcessingChainEntity> _processingChainRepository;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BranchManager"/> class.
    /// </summary>
    /// <param name="processingChainRepository">The processing chain repository.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public BranchManager(
        IMongoRepository<ProcessingChainEntity> processingChainRepository,
        ILogger logger,
        ITelemetryProvider telemetryProvider)
    {
        _processingChainRepository = processingChainRepository;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Creates branches for a flow.
    /// </summary>
    /// <param name="flow">The flow entity.</param>
    /// <param name="context">The execution context.</param>
    /// <returns>The list of branches.</returns>
    public async Task<List<ExecutionBranch>> CreateBranchesAsync(FlowEntity flow, FlowOrchestrator.ExecutionBase.Models.ExecutionContext context)
    {
        using var span = _telemetryProvider.CreateSpan("BranchManager.CreateBranches");
        span.SetAttribute("flow.id", flow.Id);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Creating branches for flow ID: {flow.Id}");

            var branches = new List<ExecutionBranch>();

            foreach (var branchConfig in flow.BranchConfigurations)
            {
                var branch = new ExecutionBranch
                {
                    BranchId = branchConfig.Key,
                    BranchName = branchConfig.Value.BranchId,
                    ProcessingChainId = branchConfig.Value.ProcessingChainId
                };

                // Validate processing chain
                var processingChain = await _processingChainRepository.GetByIdAsync(branch.ProcessingChainId);
                if (processingChain == null)
                {
                    throw new Exception($"Processing chain with ID {branch.ProcessingChainId} not found");
                }

                branches.Add(branch);
            }

            _logger.Info($"Created {branches.Count} branches for flow ID: {flow.Id}");
            span.SetStatus(SpanStatus.Ok);
            return branches;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to create branches for flow ID: {flow.Id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates branches for a flow.
    /// </summary>
    /// <param name="flow">The flow entity.</param>
    /// <param name="branches">The list of branches.</param>
    /// <returns>True if the branches are valid, false otherwise.</returns>
    public async Task<bool> ValidateBranchesAsync(FlowEntity flow, List<ExecutionBranch> branches)
    {
        using var span = _telemetryProvider.CreateSpan("BranchManager.ValidateBranches");
        span.SetAttribute("flow.id", flow.Id);

        try
        {
            _logger.Info($"Validating branches for flow ID: {flow.Id}");

            if (branches.Count != flow.BranchConfigurations.Count)
            {
                _logger.Warn($"Branch count mismatch for flow ID: {flow.Id}");
                return false;
            }

            foreach (var branch in branches)
            {
                // Validate processing chain
                var processingChain = await _processingChainRepository.GetByIdAsync(branch.ProcessingChainId);
                if (processingChain == null)
                {
                    _logger.Warn($"Processing chain with ID {branch.ProcessingChainId} not found for branch ID: {branch.BranchId}");
                    return false;
                }
            }

            _logger.Info($"Branches validated for flow ID: {flow.Id}");
            span.SetStatus(SpanStatus.Ok);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate branches for flow ID: {flow.Id}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return false;
        }
    }

    /// <summary>
    /// Executes branches in parallel.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <param name="branches">The list of branches.</param>
    /// <param name="branchExecutor">The branch executor function.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteBranchesInParallelAsync(
        FlowOrchestrator.ExecutionBase.Models.ExecutionContext context,
        List<ExecutionBranch> branches,
        Func<FlowOrchestrator.ExecutionBase.Models.ExecutionContext, ExecutionBranch, Task> branchExecutor)
    {
        using var span = _telemetryProvider.CreateSpan("BranchManager.ExecuteBranchesInParallel");
        span.SetAttribute("flow.id", context.FlowId);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Executing {branches.Count} branches in parallel for flow ID: {context.FlowId}");

            var tasks = branches.Select(branch => branchExecutor(context, branch)).ToList();
            await Task.WhenAll(tasks);

            _logger.Info($"Completed executing {branches.Count} branches in parallel for flow ID: {context.FlowId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to execute branches in parallel for flow ID: {context.FlowId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Executes branches sequentially.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <param name="branches">The list of branches.</param>
    /// <param name="branchExecutor">The branch executor function.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteBranchesSequentiallyAsync(
        FlowOrchestrator.ExecutionBase.Models.ExecutionContext context,
        List<ExecutionBranch> branches,
        Func<FlowOrchestrator.ExecutionBase.Models.ExecutionContext, ExecutionBranch, Task> branchExecutor)
    {
        using var span = _telemetryProvider.CreateSpan("BranchManager.ExecuteBranchesSequentially");
        span.SetAttribute("flow.id", context.FlowId);
        span.SetAttribute("correlation.id", context.CorrelationId);

        try
        {
            _logger.Info($"Executing {branches.Count} branches sequentially for flow ID: {context.FlowId}");

            foreach (var branch in branches)
            {
                await branchExecutor(context, branch);
            }

            _logger.Info($"Completed executing {branches.Count} branches sequentially for flow ID: {context.FlowId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to execute branches sequentially for flow ID: {context.FlowId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Services;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.ScheduledFlowEntityManager.Services;

/// <summary>
/// Service for managing scheduled flow entities.
/// </summary>
public class ScheduledFlowService : AbstractEntityService<ScheduledFlowEntity, IValidator<ScheduledFlowEntity>>
{
    // Private fields
    private readonly IMongoRepository<ScheduledFlowEntity> _repository;
    private readonly IValidator<ScheduledFlowEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly IRepository<FlowEntity> _flowRepository;
    private readonly IRepository<SourceAssignmentEntity> _sourceAssignmentRepository;
    private readonly IRepository<DestinationAssignmentEntity> _destinationAssignmentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledFlowService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="flowRepository">The flow repository.</param>
    /// <param name="sourceAssignmentRepository">The source assignment repository.</param>
    /// <param name="destinationAssignmentRepository">The destination assignment repository.</param>
    public ScheduledFlowService(
        IMongoRepository<ScheduledFlowEntity> repository,
        IValidator<ScheduledFlowEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        IRepository<FlowEntity> flowRepository,
        IRepository<SourceAssignmentEntity> sourceAssignmentRepository,
        IRepository<DestinationAssignmentEntity> destinationAssignmentRepository)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _flowRepository = flowRepository;
        _sourceAssignmentRepository = sourceAssignmentRepository;
        _destinationAssignmentRepository = destinationAssignmentRepository;
    }

    /// <summary>
    /// Gets scheduled flows by flow.
    /// </summary>
    /// <param name="flowId">The flow identifier.</param>
    /// <returns>The scheduled flows.</returns>
    public async Task<IEnumerable<ScheduledFlowEntity>> GetByFlowAsync(string flowId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ScheduledFlowEntity)}.GetByFlow");
        span.SetAttribute("flow.id", flowId);

        try
        {
            _logger.Info($"Getting {nameof(ScheduledFlowEntity)} entities with flow ID: {flowId}");
            return await _repository.GetByFilterAsync(e => e.FlowId == flowId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ScheduledFlowEntity)} entities with flow ID: {flowId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets scheduled flows by status.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns>The scheduled flows.</returns>
    public async Task<IEnumerable<ScheduledFlowEntity>> GetByStatusAsync(string status)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ScheduledFlowEntity)}.GetByStatus");
        span.SetAttribute("status", status);

        try
        {
            _logger.Info($"Getting {nameof(ScheduledFlowEntity)} entities with status: {status}");
            return await _repository.GetByFilterAsync(e => e.Status == status);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ScheduledFlowEntity)} entities with status: {status}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates the compatibility between flow, source assignment, and destination assignments.
    /// </summary>
    /// <param name="flowId">The flow identifier.</param>
    /// <param name="sourceAssignmentId">The source assignment identifier.</param>
    /// <param name="destinationAssignmentIds">The destination assignment identifiers.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateCompatibilityAsync(
        string flowId,
        string sourceAssignmentId,
        List<string> destinationAssignmentIds)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ScheduledFlowEntity)}.ValidateCompatibility");
        span.SetAttribute("flow.id", flowId);
        span.SetAttribute("source.assignment.id", sourceAssignmentId);

        try
        {
            _logger.Info($"Validating compatibility between flow {flowId}, source assignment {sourceAssignmentId}, and destination assignments");

            var result = new ValidationResult();

            // Validate that flow, source assignment, and destination assignments exist
            var flow = await _flowRepository.GetByIdAsync(flowId);
            var sourceAssignment = await _sourceAssignmentRepository.GetByIdAsync(sourceAssignmentId);

            if (flow == null)
            {
                result.AddError("FlowId", $"Flow with ID {flowId} not found.");
            }

            if (sourceAssignment == null)
            {
                result.AddError("SourceAssignmentId", $"Source assignment with ID {sourceAssignmentId} not found.");
            }

            foreach (var destinationAssignmentId in destinationAssignmentIds)
            {
                var destinationAssignment = await _destinationAssignmentRepository.GetByIdAsync(destinationAssignmentId);
                if (destinationAssignment == null)
                {
                    result.AddError("DestinationAssignmentIds", $"Destination assignment with ID {destinationAssignmentId} not found.");
                }
            }

            // If any entities don't exist, return early
            if (!result.IsValid)
            {
                return result;
            }

            // Validate that the flow is active
            if (flow.Status != "Active")
            {
                result.AddError("FlowId", $"Flow {flow.Name} is not active.");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ScheduledFlowEntityManager.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.ScheduledFlowEntityManager.Controllers;

/// <summary>
/// Controller for managing scheduled flow entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScheduledFlowController : AbstractEntityController<ScheduledFlowEntity, IValidator<ScheduledFlowEntity>>
{
    // Private fields
    private readonly IMongoRepository<ScheduledFlowEntity> _repository;
    private readonly IValidator<ScheduledFlowEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly ScheduledFlowService _scheduledFlowService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledFlowController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="scheduledFlowService">The scheduled flow service.</param>
    public ScheduledFlowController(
        IMongoRepository<ScheduledFlowEntity> repository,
        IValidator<ScheduledFlowEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        ScheduledFlowService scheduledFlowService)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _scheduledFlowService = scheduledFlowService;
    }

    /// <summary>
    /// Gets scheduled flows by flow.
    /// </summary>
    /// <param name="flowId">The flow identifier.</param>
    /// <returns>The scheduled flows.</returns>
    [HttpGet("byflow/{flowId}")]
    public async Task<ActionResult<IEnumerable<ScheduledFlowEntity>>> GetByFlow(string flowId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ScheduledFlowEntity)}.GetByFlow");
        span.SetAttribute("flow.id", flowId);

        try
        {
            _logger.Info($"Getting {nameof(ScheduledFlowEntity)} entities with flow ID: {flowId}");
            var entities = await _scheduledFlowService.GetByFlowAsync(flowId);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ScheduledFlowEntity)} entities with flow ID: {flowId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets scheduled flows by status.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <returns>The scheduled flows.</returns>
    [HttpGet("bystatus/{status}")]
    public async Task<ActionResult<IEnumerable<ScheduledFlowEntity>>> GetByStatus(string status)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ScheduledFlowEntity)}.GetByStatus");
        span.SetAttribute("status", status);

        try
        {
            _logger.Info($"Getting {nameof(ScheduledFlowEntity)} entities with status: {status}");
            var entities = await _scheduledFlowService.GetByStatusAsync(status);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ScheduledFlowEntity)} entities with status: {status}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates the compatibility between flow, source assignment, and destination assignments.
    /// </summary>
    /// <param name="flowId">The flow identifier.</param>
    /// <param name="sourceAssignmentId">The source assignment identifier.</param>
    /// <param name="destinationAssignmentIds">The destination assignment identifiers.</param>
    /// <returns>The validation result.</returns>
    [HttpGet("validatecompatibility")]
    public async Task<ActionResult<ValidationResult>> ValidateCompatibility(
        [FromQuery] string flowId,
        [FromQuery] string sourceAssignmentId,
        [FromQuery] List<string> destinationAssignmentIds)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ScheduledFlowEntity)}.ValidateCompatibility");
        span.SetAttribute("flow.id", flowId);
        span.SetAttribute("source.assignment.id", sourceAssignmentId);

        try
        {
            _logger.Info($"Validating compatibility between flow {flowId}, source assignment {sourceAssignmentId}, and destination assignments");
            var result = await _scheduledFlowService.ValidateCompatibilityAsync(flowId, sourceAssignmentId, destinationAssignmentIds);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

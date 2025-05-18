using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.DestinationAssignmentEntityManager.Services;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.DestinationAssignmentEntityManager.Controllers;

/// <summary>
/// Controller for managing destination assignment entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DestinationAssignmentController : AbstractEntityController<DestinationAssignmentEntity, IValidator<DestinationAssignmentEntity>>
{
    // Private fields
    private readonly IMongoRepository<DestinationAssignmentEntity> _repository;
    private readonly IValidator<DestinationAssignmentEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly DestinationAssignmentService _destinationAssignmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DestinationAssignmentController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="destinationAssignmentService">The destination assignment service.</param>
    public DestinationAssignmentController(
        IMongoRepository<DestinationAssignmentEntity> repository,
        IValidator<DestinationAssignmentEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        DestinationAssignmentService destinationAssignmentService)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _destinationAssignmentService = destinationAssignmentService;
    }

    /// <summary>
    /// Gets destination assignments by destination.
    /// </summary>
    /// <param name="destinationId">The destination identifier.</param>
    /// <returns>The destination assignments.</returns>
    [HttpGet("bydestination/{destinationId}")]
    public async Task<ActionResult<IEnumerable<DestinationAssignmentEntity>>> GetByDestination(string destinationId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(DestinationAssignmentEntity)}.GetByDestination");
        span.SetAttribute("destination.id", destinationId);

        try
        {
            _logger.Info($"Getting {nameof(DestinationAssignmentEntity)} entities with destination ID: {destinationId}");
            var entities = await _destinationAssignmentService.GetByDestinationAsync(destinationId);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(DestinationAssignmentEntity)} entities with destination ID: {destinationId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets destination assignments by exporter.
    /// </summary>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <returns>The destination assignments.</returns>
    [HttpGet("byexporter/{exporterId}")]
    public async Task<ActionResult<IEnumerable<DestinationAssignmentEntity>>> GetByExporter(string exporterId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(DestinationAssignmentEntity)}.GetByExporter");
        span.SetAttribute("exporter.id", exporterId);

        try
        {
            _logger.Info($"Getting {nameof(DestinationAssignmentEntity)} entities with exporter ID: {exporterId}");
            var entities = await _destinationAssignmentService.GetByExporterAsync(exporterId);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(DestinationAssignmentEntity)} entities with exporter ID: {exporterId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates destination-exporter compatibility.
    /// </summary>
    /// <param name="destinationId">The destination identifier.</param>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <returns>The validation result.</returns>
    [HttpGet("validatecompatibility")]
    public async Task<ActionResult<ValidationResult>> ValidateCompatibility(
        [FromQuery] string destinationId,
        [FromQuery] string exporterId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(DestinationAssignmentEntity)}.ValidateCompatibility");
        span.SetAttribute("destination.id", destinationId);
        span.SetAttribute("exporter.id", exporterId);

        try
        {
            _logger.Info($"Validating compatibility between destination {destinationId} and exporter {exporterId}");
            var result = await _destinationAssignmentService.ValidateCompatibilityAsync(destinationId, exporterId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility between destination {destinationId} and exporter {exporterId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

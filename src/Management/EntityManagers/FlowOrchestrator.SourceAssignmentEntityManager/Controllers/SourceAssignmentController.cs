using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.SourceAssignmentEntityManager.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.SourceAssignmentEntityManager.Controllers;

/// <summary>
/// Controller for managing source assignment entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SourceAssignmentController : AbstractEntityController<SourceAssignmentEntity, IValidator<SourceAssignmentEntity>>
{
    // Private fields
    private readonly IMongoRepository<SourceAssignmentEntity> _repository;
    private readonly IValidator<SourceAssignmentEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly SourceAssignmentService _sourceAssignmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceAssignmentController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="sourceAssignmentService">The source assignment service.</param>
    public SourceAssignmentController(
        IMongoRepository<SourceAssignmentEntity> repository,
        IValidator<SourceAssignmentEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        SourceAssignmentService sourceAssignmentService)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _sourceAssignmentService = sourceAssignmentService;
    }

    /// <summary>
    /// Gets source assignments by source.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <returns>The source assignments.</returns>
    [HttpGet("bysource/{sourceId}")]
    public async Task<ActionResult<IEnumerable<SourceAssignmentEntity>>> GetBySource(string sourceId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(SourceAssignmentEntity)}.GetBySource");
        span.SetAttribute("source.id", sourceId);

        try
        {
            _logger.Info($"Getting {nameof(SourceAssignmentEntity)} entities with source ID: {sourceId}");
            var entities = await _sourceAssignmentService.GetBySourceAsync(sourceId);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(SourceAssignmentEntity)} entities with source ID: {sourceId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets source assignments by importer.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <returns>The source assignments.</returns>
    [HttpGet("byimporter/{importerId}")]
    public async Task<ActionResult<IEnumerable<SourceAssignmentEntity>>> GetByImporter(string importerId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(SourceAssignmentEntity)}.GetByImporter");
        span.SetAttribute("importer.id", importerId);

        try
        {
            _logger.Info($"Getting {nameof(SourceAssignmentEntity)} entities with importer ID: {importerId}");
            var entities = await _sourceAssignmentService.GetByImporterAsync(importerId);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(SourceAssignmentEntity)} entities with importer ID: {importerId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates source-importer compatibility.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <param name="importerId">The importer identifier.</param>
    /// <returns>The validation result.</returns>
    [HttpGet("validatecompatibility")]
    public async Task<ActionResult<ValidationResult>> ValidateCompatibility(
        [FromQuery] string sourceId,
        [FromQuery] string importerId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(SourceAssignmentEntity)}.ValidateCompatibility");
        span.SetAttribute("source.id", sourceId);
        span.SetAttribute("importer.id", importerId);

        try
        {
            _logger.Info($"Validating compatibility between source {sourceId} and importer {importerId}");
            var result = await _sourceAssignmentService.ValidateCompatibilityAsync(sourceId, importerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility between source {sourceId} and importer {importerId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.FlowEntityManager.Services;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.FlowEntityManager.Controllers;

/// <summary>
/// Controller for managing flow entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FlowController : AbstractEntityController<FlowEntity, IValidator<FlowEntity>>
{
    // Private fields
    private readonly IMongoRepository<FlowEntity> _repository;
    private readonly IValidator<FlowEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly FlowService _flowService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="flowService">The flow service.</param>
    public FlowController(
        IMongoRepository<FlowEntity> repository,
        IValidator<FlowEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        FlowService flowService)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _flowService = flowService;
    }

    /// <summary>
    /// Gets flows by status.
    /// </summary>
    /// <param name="status">The flow status.</param>
    /// <returns>The flows.</returns>
    [HttpGet("bystatus/{status}")]
    public async Task<ActionResult<IEnumerable<FlowEntity>>> GetByStatus(string status)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(FlowEntity)}.GetByStatus");
        span.SetAttribute("flow.status", status);

        try
        {
            _logger.Info($"Getting {nameof(FlowEntity)} entities with status: {status}");
            var entities = await _flowService.GetByStatusAsync(status);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(FlowEntity)} entities with status: {status}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets flows by tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <returns>The flows.</returns>
    [HttpGet("bytag/{tag}")]
    public async Task<ActionResult<IEnumerable<FlowEntity>>> GetByTag(string tag)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(FlowEntity)}.GetByTag");
        span.SetAttribute("flow.tag", tag);

        try
        {
            _logger.Info($"Getting {nameof(FlowEntity)} entities with tag: {tag}");
            var entities = await _flowService.GetByTagAsync(tag);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(FlowEntity)} entities with tag: {tag}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates the compatibility between importers, processing chains, and exporters.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <param name="processingChainId">The processing chain identifier.</param>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <returns>The validation result.</returns>
    [HttpGet("validatecompatibility")]
    public async Task<ActionResult<ValidationResult>> ValidateCompatibility(
        [FromQuery] string importerId,
        [FromQuery] string processingChainId,
        [FromQuery] string exporterId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(FlowEntity)}.ValidateCompatibility");
        span.SetAttribute("importer.id", importerId);
        span.SetAttribute("processingchain.id", processingChainId);
        span.SetAttribute("exporter.id", exporterId);

        try
        {
            _logger.Info($"Validating compatibility between importer {importerId}, processing chain {processingChainId}, and exporter {exporterId}");
            var result = await _flowService.ValidateCompatibilityAsync(importerId, processingChainId, exporterId);
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

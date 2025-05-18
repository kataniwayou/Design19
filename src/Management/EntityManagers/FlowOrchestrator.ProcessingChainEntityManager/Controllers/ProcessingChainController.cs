using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ProcessingChainEntityManager.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.ProcessingChainEntityManager.Controllers;

/// <summary>
/// Controller for managing processing chain entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProcessingChainController : AbstractEntityController<ProcessingChainEntity, IValidator<ProcessingChainEntity>>
{
    // Private fields
    private readonly IMongoRepository<ProcessingChainEntity> _repository;
    private readonly IValidator<ProcessingChainEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly ProcessingChainService _processingChainService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingChainController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="processingChainService">The processing chain service.</param>
    public ProcessingChainController(
        IMongoRepository<ProcessingChainEntity> repository,
        IValidator<ProcessingChainEntity> validator,
        ILogger logger,
        ITelemetryProvider telemetryProvider,
        ProcessingChainService processingChainService)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _processingChainService = processingChainService;
    }

    /// <summary>
    /// Gets processing chains by type.
    /// </summary>
    /// <param name="type">The processing chain type.</param>
    /// <returns>The processing chains.</returns>
    [HttpGet("bytype/{type}")]
    public async Task<ActionResult<IEnumerable<ProcessingChainEntity>>> GetByType(string type)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessingChainEntity)}.GetByType");
        span.SetAttribute("processingchain.type", type);

        try
        {
            _logger.Info($"Getting {nameof(ProcessingChainEntity)} entities with type: {type}");
            var entities = await _processingChainService.GetByTypeAsync(type);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessingChainEntity)} entities with type: {type}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets processing chains by processor.
    /// </summary>
    /// <param name="processorId">The processor identifier.</param>
    /// <returns>The processing chains.</returns>
    [HttpGet("byprocessor/{processorId}")]
    public async Task<ActionResult<IEnumerable<ProcessingChainEntity>>> GetByProcessor(string processorId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessingChainEntity)}.GetByProcessor");
        span.SetAttribute("processor.id", processorId);

        try
        {
            _logger.Info($"Getting {nameof(ProcessingChainEntity)} entities with processor ID: {processorId}");
            var entities = await _processingChainService.GetByProcessorAsync(processorId);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessingChainEntity)} entities with processor ID: {processorId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates processor compatibility within a chain.
    /// </summary>
    /// <param name="processorIds">The processor identifiers.</param>
    /// <returns>The validation result.</returns>
    [HttpPost("validateprocessorcompatibility")]
    public async Task<ActionResult<ValidationResult>> ValidateProcessorCompatibility([FromBody] List<string> processorIds)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessingChainEntity)}.ValidateProcessorCompatibility");

        try
        {
            _logger.Info($"Validating processor compatibility for {processorIds.Count} processors");
            var result = await _processingChainService.ValidateProcessorCompatibilityAsync(processorIds);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate processor compatibility", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

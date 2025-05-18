using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using Microsoft.AspNetCore.Mvc;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.ProcessorEntityManager.Controllers;

/// <summary>
/// Controller for managing processor entities.
/// </summary>
public class ProcessorController : AbstractEntityController<ProcessorEntity, IValidator<ProcessorEntity>>
{
    // Private fields
    private readonly IMongoRepository<ProcessorEntity> _repository;
    private readonly IValidator<ProcessorEntity> _validator;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public ProcessorController(
        IMongoRepository<ProcessorEntity> repository,
        IValidator<ProcessorEntity> validator,
        ILogger logger,
        ITelemetryProvider telemetryProvider)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Gets processors by type.
    /// </summary>
    /// <param name="type">The processor type.</param>
    /// <returns>The processors.</returns>
    [HttpGet("bytype/{type}")]
    public async Task<ActionResult<IEnumerable<ProcessorEntity>>> GetByType(string type)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessorEntity)}.GetByType");
        span.SetAttribute("processor.type", type);

        try
        {
            _logger.Info($"Getting {nameof(ProcessorEntity)} entities with type: {type}");
            var entities = await _repository.GetByFilterAsync(e => e.ProcessorType == type);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessorEntity)} entities with type: {type}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets processors by capability.
    /// </summary>
    /// <param name="capability">The capability.</param>
    /// <returns>The processors.</returns>
    [HttpGet("bycapability/{capability}")]
    public async Task<ActionResult<IEnumerable<ProcessorEntity>>> GetByCapability(string capability)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessorEntity)}.GetByCapability");
        span.SetAttribute("processor.capability", capability);

        try
        {
            _logger.Info($"Getting {nameof(ProcessorEntity)} entities with capability: {capability}");
            var entities = await _repository.GetByFilterAsync(e => e.Capabilities.Contains(capability));
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessorEntity)} entities with capability: {capability}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

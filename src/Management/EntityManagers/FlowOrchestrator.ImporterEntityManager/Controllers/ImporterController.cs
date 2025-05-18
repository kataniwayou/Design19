using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Controllers;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using Microsoft.AspNetCore.Mvc;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.ImporterEntityManager.Controllers;

/// <summary>
/// Controller for managing importer entities.
/// </summary>
public class ImporterController : AbstractEntityController<ImporterEntity, IValidator<ImporterEntity>>
{
    // Private fields
    private readonly IMongoRepository<ImporterEntity> _repository;
    private readonly IValidator<ImporterEntity> _validator;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImporterController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public ImporterController(
        IMongoRepository<ImporterEntity> repository,
        IValidator<ImporterEntity> validator,
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
    /// Gets importers by type.
    /// </summary>
    /// <param name="type">The importer type.</param>
    /// <returns>The importers.</returns>
    [HttpGet("bytype/{type}")]
    public async Task<ActionResult<IEnumerable<ImporterEntity>>> GetByType(string type)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ImporterEntity)}.GetByType");
        span.SetAttribute("importer.type", type);

        try
        {
            _logger.Info($"Getting {nameof(ImporterEntity)} entities with type: {type}");
            var entities = await _repository.GetByFilterAsync(e => e.ImporterType == type);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ImporterEntity)} entities with type: {type}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets importers by protocol.
    /// </summary>
    /// <param name="protocol">The protocol.</param>
    /// <returns>The importers.</returns>
    [HttpGet("byprotocol/{protocol}")]
    public async Task<ActionResult<IEnumerable<ImporterEntity>>> GetByProtocol(string protocol)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ImporterEntity)}.GetByProtocol");
        span.SetAttribute("importer.protocol", protocol);

        try
        {
            _logger.Info($"Getting {nameof(ImporterEntity)} entities with protocol: {protocol}");
            var entities = await _repository.GetByFilterAsync(e => e.Protocol == protocol);
            return Ok(entities);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ImporterEntity)} entities with protocol: {protocol}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

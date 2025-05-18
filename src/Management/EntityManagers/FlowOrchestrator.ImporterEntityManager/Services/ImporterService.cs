using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Services;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.ImporterEntityManager.Services;

/// <summary>
/// Service for managing importer entities.
/// </summary>
public class ImporterService : AbstractEntityService<ImporterEntity, IValidator<ImporterEntity>>
{
    // Private fields
    private readonly IMongoRepository<ImporterEntity> _repository;
    private readonly IValidator<ImporterEntity> _validator;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImporterService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public ImporterService(
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
    public async Task<IEnumerable<ImporterEntity>> GetByTypeAsync(string type)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ImporterEntity)}.GetByType");
        span.SetAttribute("importer.type", type);

        try
        {
            _logger.Info($"Getting {nameof(ImporterEntity)} entities with type: {type}");
            return await _repository.GetByFilterAsync(e => e.ImporterType == type);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ImporterEntity)} entities with type: {type}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets importers by protocol.
    /// </summary>
    /// <param name="protocol">The protocol.</param>
    /// <returns>The importers.</returns>
    public async Task<IEnumerable<ImporterEntity>> GetByProtocolAsync(string protocol)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ImporterEntity)}.GetByProtocol");
        span.SetAttribute("importer.protocol", protocol);

        try
        {
            _logger.Info($"Getting {nameof(ImporterEntity)} entities with protocol: {protocol}");
            return await _repository.GetByFilterAsync(e => e.Protocol == protocol);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ImporterEntity)} entities with protocol: {protocol}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

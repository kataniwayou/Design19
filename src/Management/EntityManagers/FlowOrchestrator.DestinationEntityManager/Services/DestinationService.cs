using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Services;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;

namespace FlowOrchestrator.DestinationEntityManager.Services;

/// <summary>
/// Service for managing destination entities.
/// </summary>
public class DestinationService : AbstractEntityService<DestinationEntity, IValidator<DestinationEntity>>
{
    // Private fields
    private readonly IMongoRepository<DestinationEntity> _repository;
    private readonly IValidator<DestinationEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DestinationService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public DestinationService(
        IMongoRepository<DestinationEntity> repository,
        IValidator<DestinationEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }
}

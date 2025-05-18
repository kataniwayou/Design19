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

namespace FlowOrchestrator.DestinationAssignmentEntityManager.Services;

/// <summary>
/// Service for managing destination assignment entities.
/// </summary>
public class DestinationAssignmentService : AbstractEntityService<DestinationAssignmentEntity, IValidator<DestinationAssignmentEntity>>
{
    // Private fields
    private readonly IMongoRepository<DestinationAssignmentEntity> _repository;
    private readonly IValidator<DestinationAssignmentEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly IRepository<DestinationEntity> _destinationRepository;
    private readonly IRepository<ExporterEntity> _exporterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DestinationAssignmentService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="destinationRepository">The destination repository.</param>
    /// <param name="exporterRepository">The exporter repository.</param>
    public DestinationAssignmentService(
        IMongoRepository<DestinationAssignmentEntity> repository,
        IValidator<DestinationAssignmentEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        IRepository<DestinationEntity> destinationRepository,
        IRepository<ExporterEntity> exporterRepository)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _destinationRepository = destinationRepository;
        _exporterRepository = exporterRepository;
    }

    /// <summary>
    /// Gets destination assignments by destination.
    /// </summary>
    /// <param name="destinationId">The destination identifier.</param>
    /// <returns>The destination assignments.</returns>
    public async Task<IEnumerable<DestinationAssignmentEntity>> GetByDestinationAsync(string destinationId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(DestinationAssignmentEntity)}.GetByDestination");
        span.SetAttribute("destination.id", destinationId);

        try
        {
            _logger.Info($"Getting {nameof(DestinationAssignmentEntity)} entities with destination ID: {destinationId}");
            return await _repository.GetByFilterAsync(e => e.DestinationId == destinationId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(DestinationAssignmentEntity)} entities with destination ID: {destinationId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets destination assignments by exporter.
    /// </summary>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <returns>The destination assignments.</returns>
    public async Task<IEnumerable<DestinationAssignmentEntity>> GetByExporterAsync(string exporterId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(DestinationAssignmentEntity)}.GetByExporter");
        span.SetAttribute("exporter.id", exporterId);

        try
        {
            _logger.Info($"Getting {nameof(DestinationAssignmentEntity)} entities with exporter ID: {exporterId}");
            return await _repository.GetByFilterAsync(e => e.ExporterId == exporterId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(DestinationAssignmentEntity)} entities with exporter ID: {exporterId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates destination-exporter compatibility.
    /// </summary>
    /// <param name="destinationId">The destination identifier.</param>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateCompatibilityAsync(string destinationId, string exporterId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(DestinationAssignmentEntity)}.ValidateCompatibility");
        span.SetAttribute("destination.id", destinationId);
        span.SetAttribute("exporter.id", exporterId);

        try
        {
            _logger.Info($"Validating compatibility between destination {destinationId} and exporter {exporterId}");

            var result = new ValidationResult();

            // Validate that destination and exporter exist
            var destination = await _destinationRepository.GetByIdAsync(destinationId);
            var exporter = await _exporterRepository.GetByIdAsync(exporterId);

            if (destination == null)
            {
                result.AddError("DestinationId", $"Destination with ID {destinationId} not found.");
            }

            if (exporter == null)
            {
                result.AddError("ExporterId", $"Exporter with ID {exporterId} not found.");
            }

            // If destination or exporter don't exist, return early
            if (!result.IsValid)
            {
                return result;
            }

            // Validate destination-exporter compatibility
            if (!IsDestinationCompatibleWithExporter(destination, exporter))
            {
                result.AddError("Compatibility",
                    $"Destination {destination.Name} is not compatible with exporter {exporter.Name}.");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility between destination {destinationId} and exporter {exporterId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private bool IsDestinationCompatibleWithExporter(DestinationEntity destination, ExporterEntity exporter)
    {
        // Check if the exporter supports the destination's protocol
        if (!string.IsNullOrEmpty(destination.DataFormat) &&
            !exporter.Capabilities.Contains(destination.DataFormat))
        {
            return false;
        }

        // Check if the exporter supports any of the destination's protocols
        if (destination.SupportedProtocols.Count > 0 &&
            !destination.SupportedProtocols.Contains(exporter.Protocol))
        {
            return false;
        }

        return true;
    }
}

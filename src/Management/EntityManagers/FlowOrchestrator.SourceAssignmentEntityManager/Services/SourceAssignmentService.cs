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

namespace FlowOrchestrator.SourceAssignmentEntityManager.Services;

/// <summary>
/// Service for managing source assignment entities.
/// </summary>
public class SourceAssignmentService : AbstractEntityService<SourceAssignmentEntity, IValidator<SourceAssignmentEntity>>
{
    // Private fields
    private readonly IMongoRepository<SourceAssignmentEntity> _repository;
    private readonly IValidator<SourceAssignmentEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly IRepository<SourceEntity> _sourceRepository;
    private readonly IRepository<ImporterEntity> _importerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceAssignmentService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="sourceRepository">The source repository.</param>
    /// <param name="importerRepository">The importer repository.</param>
    public SourceAssignmentService(
        IMongoRepository<SourceAssignmentEntity> repository,
        IValidator<SourceAssignmentEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        IRepository<SourceEntity> sourceRepository,
        IRepository<ImporterEntity> importerRepository)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _sourceRepository = sourceRepository;
        _importerRepository = importerRepository;
    }

    /// <summary>
    /// Gets source assignments by source.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <returns>The source assignments.</returns>
    public async Task<IEnumerable<SourceAssignmentEntity>> GetBySourceAsync(string sourceId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(SourceAssignmentEntity)}.GetBySource");
        span.SetAttribute("source.id", sourceId);

        try
        {
            _logger.Info($"Getting {nameof(SourceAssignmentEntity)} entities with source ID: {sourceId}");
            return await _repository.GetByFilterAsync(e => e.SourceId == sourceId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(SourceAssignmentEntity)} entities with source ID: {sourceId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets source assignments by importer.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <returns>The source assignments.</returns>
    public async Task<IEnumerable<SourceAssignmentEntity>> GetByImporterAsync(string importerId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(SourceAssignmentEntity)}.GetByImporter");
        span.SetAttribute("importer.id", importerId);

        try
        {
            _logger.Info($"Getting {nameof(SourceAssignmentEntity)} entities with importer ID: {importerId}");
            return await _repository.GetByFilterAsync(e => e.ImporterId == importerId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(SourceAssignmentEntity)} entities with importer ID: {importerId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates source-importer compatibility.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <param name="importerId">The importer identifier.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateCompatibilityAsync(string sourceId, string importerId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(SourceAssignmentEntity)}.ValidateCompatibility");
        span.SetAttribute("source.id", sourceId);
        span.SetAttribute("importer.id", importerId);

        try
        {
            _logger.Info($"Validating compatibility between source {sourceId} and importer {importerId}");

            var result = new ValidationResult();

            // Validate that source and importer exist
            var source = await _sourceRepository.GetByIdAsync(sourceId);
            var importer = await _importerRepository.GetByIdAsync(importerId);

            if (source == null)
            {
                result.AddError("SourceId", $"Source with ID {sourceId} not found.");
            }

            if (importer == null)
            {
                result.AddError("ImporterId", $"Importer with ID {importerId} not found.");
            }

            // If source or importer don't exist, return early
            if (!result.IsValid)
            {
                return result;
            }

            // Validate source-importer compatibility
            if (!IsSourceCompatibleWithImporter(source, importer))
            {
                result.AddError("Compatibility",
                    $"Source {source.Name} is not compatible with importer {importer.Name}.");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility between source {sourceId} and importer {importerId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private bool IsSourceCompatibleWithImporter(SourceEntity source, ImporterEntity importer)
    {
        // Check if the importer supports the source's protocol
        if (!string.IsNullOrEmpty(source.DataFormat) &&
            !importer.Capabilities.Contains(source.DataFormat))
        {
            return false;
        }

        // Check if the importer supports any of the source's protocols
        if (source.SupportedProtocols.Count > 0 &&
            !source.SupportedProtocols.Contains(importer.Protocol))
        {
            return false;
        }

        return true;
    }
}

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

namespace FlowOrchestrator.FlowEntityManager.Services;

/// <summary>
/// Service for managing flow entities.
/// </summary>
public class FlowService : AbstractEntityService<FlowEntity, IValidator<FlowEntity>>
{
    // Private fields
    private readonly IMongoRepository<FlowEntity> _repository;
    private readonly IValidator<FlowEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly FlowValidator _flowValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="flowValidator">The flow validator.</param>
    public FlowService(
        IMongoRepository<FlowEntity> repository,
        IValidator<FlowEntity> validator,
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        FlowValidator flowValidator)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _flowValidator = flowValidator;
    }

    /// <summary>
    /// Gets flows by status.
    /// </summary>
    /// <param name="status">The flow status.</param>
    /// <returns>The flows.</returns>
    public async Task<IEnumerable<FlowEntity>> GetByStatusAsync(string status)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(FlowEntity)}.GetByStatus");
        span.SetAttribute("flow.status", status);

        try
        {
            _logger.Info($"Getting {nameof(FlowEntity)} entities with status: {status}");
            return await _repository.GetByFilterAsync(e => e.Status == status);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(FlowEntity)} entities with status: {status}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets flows by tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <returns>The flows.</returns>
    public async Task<IEnumerable<FlowEntity>> GetByTagAsync(string tag)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(FlowEntity)}.GetByTag");
        span.SetAttribute("flow.tag", tag);

        try
        {
            _logger.Info($"Getting {nameof(FlowEntity)} entities with tag: {tag}");
            return await _repository.GetByFilterAsync(e => e.Tags.Contains(tag));
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(FlowEntity)} entities with tag: {tag}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates the compatibility between importers, processing chains, and exporters.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <param name="processingChainId">The processing chain identifier.</param>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateCompatibilityAsync(string importerId, string processingChainId, string exporterId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(FlowEntity)}.ValidateCompatibility");
        span.SetAttribute("importer.id", importerId);
        span.SetAttribute("processingchain.id", processingChainId);
        span.SetAttribute("exporter.id", exporterId);

        try
        {
            _logger.Info($"Validating compatibility between importer {importerId}, processing chain {processingChainId}, and exporter {exporterId}");
            return await _flowValidator.ValidateCompatibilityAsync(importerId, processingChainId, exporterId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate compatibility", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

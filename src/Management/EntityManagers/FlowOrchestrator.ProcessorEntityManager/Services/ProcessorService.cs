using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Services;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.ProcessorEntityManager.Services;

/// <summary>
/// Service for managing processor entities.
/// </summary>
public class ProcessorService : AbstractEntityService<ProcessorEntity, IValidator<ProcessorEntity>>
{
    // Private fields
    private readonly IMongoRepository<ProcessorEntity> _repository;
    private readonly IValidator<ProcessorEntity> _validator;
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public ProcessorService(
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
    public async Task<IEnumerable<ProcessorEntity>> GetByTypeAsync(string type)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessorEntity)}.GetByType");
        span.SetAttribute("processor.type", type);

        try
        {
            _logger.Info($"Getting {nameof(ProcessorEntity)} entities with type: {type}");
            return await _repository.GetByFilterAsync(e => e.ProcessorType == type);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessorEntity)} entities with type: {type}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets processors by capability.
    /// </summary>
    /// <param name="capability">The capability.</param>
    /// <returns>The processors.</returns>
    public async Task<IEnumerable<ProcessorEntity>> GetByCapabilityAsync(string capability)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessorEntity)}.GetByCapability");
        span.SetAttribute("processor.capability", capability);

        try
        {
            _logger.Info($"Getting {nameof(ProcessorEntity)} entities with capability: {capability}");
            return await _repository.GetByFilterAsync(e => e.Capabilities.Contains(capability));
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessorEntity)} entities with capability: {capability}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates schema compatibility between processors.
    /// </summary>
    /// <param name="sourceProcessorId">The source processor identifier.</param>
    /// <param name="targetProcessorId">The target processor identifier.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateSchemaCompatibilityAsync(string sourceProcessorId, string targetProcessorId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessorEntity)}.ValidateSchemaCompatibility");
        span.SetAttribute("source.processor.id", sourceProcessorId);
        span.SetAttribute("target.processor.id", targetProcessorId);

        try
        {
            _logger.Info($"Validating schema compatibility between processors: {sourceProcessorId} -> {targetProcessorId}");

            var sourceProcessor = await GetByIdAsync(sourceProcessorId);
            var targetProcessor = await GetByIdAsync(targetProcessorId);

            var result = new ValidationResult();

            if (sourceProcessor == null)
            {
                result.AddError("sourceProcessorId", $"Source processor with ID {sourceProcessorId} not found.");
                return result;
            }

            if (targetProcessor == null)
            {
                result.AddError("targetProcessorId", $"Target processor with ID {targetProcessorId} not found.");
                return result;
            }

            // Validate schema compatibility
            return ValidateSchemaCompatibility(sourceProcessor.OutputSchema, targetProcessor.InputSchema);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate schema compatibility between processors: {sourceProcessorId} -> {targetProcessorId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private ValidationResult ValidateSchemaCompatibility(SchemaDefinition sourceSchema, SchemaDefinition targetSchema)
    {
        var result = new ValidationResult();

        // Check if all required fields in the target schema are present in the source schema
        foreach (var targetField in targetSchema.Fields.Where(f => f.IsRequired))
        {
            var sourceField = sourceSchema.Fields.FirstOrDefault(f => f.Name == targetField.Name);

            if (sourceField == null)
            {
                result.AddError("SchemaCompatibility", $"Required field '{targetField.Name}' in target schema is not present in source schema.");
                continue;
            }

            if (sourceField.DataType != targetField.DataType)
            {
                result.AddError("SchemaCompatibility", $"Field '{targetField.Name}' has incompatible data types: {sourceField.DataType} -> {targetField.DataType}.");
            }
        }

        return result;
    }
}

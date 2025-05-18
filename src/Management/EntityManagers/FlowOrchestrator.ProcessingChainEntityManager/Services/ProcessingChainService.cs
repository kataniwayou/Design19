using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.EntityManagerBase.Services;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowOrchestrator.ProcessingChainEntityManager.Services;

/// <summary>
/// Service for managing processing chain entities.
/// </summary>
public class ProcessingChainService : AbstractEntityService<ProcessingChainEntity, IValidator<ProcessingChainEntity>>
{
    // Private fields
    private readonly IMongoRepository<ProcessingChainEntity> _repository;
    private readonly IValidator<ProcessingChainEntity> _validator;
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly IRepository<ProcessorEntity> _processorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingChainService"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="processorRepository">The processor repository.</param>
    public ProcessingChainService(
        IMongoRepository<ProcessingChainEntity> repository,
        IValidator<ProcessingChainEntity> validator,
        ILogger logger,
        ITelemetryProvider telemetryProvider,
        IRepository<ProcessorEntity> processorRepository)
        : base(repository, validator, logger, telemetryProvider)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _processorRepository = processorRepository;
    }

    /// <summary>
    /// Gets processing chains by type.
    /// </summary>
    /// <param name="type">The processing chain type.</param>
    /// <returns>The processing chains.</returns>
    public async Task<IEnumerable<ProcessingChainEntity>> GetByTypeAsync(string type)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessingChainEntity)}.GetByType");
        span.SetAttribute("processingchain.type", type);

        try
        {
            _logger.Info($"Getting {nameof(ProcessingChainEntity)} entities with type: {type}");
            return await _repository.GetByFilterAsync(e => e.ChainType == type);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessingChainEntity)} entities with type: {type}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets processing chains by processor.
    /// </summary>
    /// <param name="processorId">The processor identifier.</param>
    /// <returns>The processing chains.</returns>
    public async Task<IEnumerable<ProcessingChainEntity>> GetByProcessorAsync(string processorId)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessingChainEntity)}.GetByProcessor");
        span.SetAttribute("processor.id", processorId);

        try
        {
            _logger.Info($"Getting {nameof(ProcessingChainEntity)} entities with processor ID: {processorId}");
            return await _repository.GetByFilterAsync(e => e.ProcessorIds.Contains(processorId));
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to get {nameof(ProcessingChainEntity)} entities with processor ID: {processorId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Validates processor compatibility within a chain.
    /// </summary>
    /// <param name="processorIds">The processor identifiers.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateProcessorCompatibilityAsync(List<string> processorIds)
    {
        using var span = _telemetryProvider.CreateSpan($"{nameof(ProcessingChainEntity)}.ValidateProcessorCompatibility");

        try
        {
            _logger.Info($"Validating processor compatibility for {processorIds.Count} processors");

            var result = new ValidationResult();

            // Validate that all processors exist
            foreach (var processorId in processorIds)
            {
                var processor = await _processorRepository.GetByIdAsync(processorId);
                if (processor == null)
                {
                    result.AddError("ProcessorIds", $"Processor with ID {processorId} not found.");
                }
            }

            // If any processors don't exist, return early
            if (!result.IsValid)
            {
                return result;
            }

            // For a chain with a single processor, no compatibility check is needed
            if (processorIds.Count == 1)
            {
                return result;
            }

            // Check compatibility between adjacent processors in the chain
            for (int i = 0; i < processorIds.Count - 1; i++)
            {
                var sourceProcessorId = processorIds[i];
                var targetProcessorId = processorIds[i + 1];

                var sourceProcessor = await _processorRepository.GetByIdAsync(sourceProcessorId);
                var targetProcessor = await _processorRepository.GetByIdAsync(targetProcessorId);

                // Validate schema compatibility
                if (!AreSchemaCompatible(sourceProcessor.OutputSchema, targetProcessor.InputSchema))
                {
                    result.AddError("ProcessorIds",
                        $"Output schema of processor {sourceProcessor.Name} is not compatible with input schema of processor {targetProcessor.Name}.");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to validate processor compatibility", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    private bool AreSchemaCompatible(SchemaDefinition sourceSchema, SchemaDefinition targetSchema)
    {
        // If target schema has no fields, it accepts any input
        if (targetSchema.Fields.Count == 0)
        {
            return true;
        }

        // Check if all required fields in the target schema are present in the source schema
        foreach (var targetField in targetSchema.Fields)
        {
            if (!targetField.IsRequired)
            {
                continue;
            }

            var sourceField = sourceSchema.Fields.FirstOrDefault(f => f.Name == targetField.Name);
            if (sourceField == null)
            {
                return false;
            }

            // Check data type compatibility
            if (!AreDataTypesCompatible(sourceField.DataType, targetField.DataType))
            {
                return false;
            }
        }

        return true;
    }

    private bool AreDataTypesCompatible(string sourceType, string targetType)
    {
        // If types are the same, they are compatible
        if (sourceType == targetType)
        {
            return true;
        }

        // Define type compatibility rules
        switch (targetType.ToLowerInvariant())
        {
            case "string":
                // Any type can be converted to string
                return true;
            case "number":
            case "decimal":
            case "double":
            case "float":
                // Integer types can be converted to floating-point types
                return sourceType.ToLowerInvariant() == "integer" ||
                       sourceType.ToLowerInvariant() == "int" ||
                       sourceType.ToLowerInvariant() == "long";
            case "boolean":
                // Only boolean types are compatible with boolean
                return sourceType.ToLowerInvariant() == "boolean" ||
                       sourceType.ToLowerInvariant() == "bool";
            case "date":
            case "datetime":
                // String can be converted to date/datetime if it's in the correct format
                return sourceType.ToLowerInvariant() == "string" ||
                       sourceType.ToLowerInvariant() == "date" ||
                       sourceType.ToLowerInvariant() == "datetime";
            default:
                // For other types, require exact match
                return false;
        }
    }
}

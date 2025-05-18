using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Domain.Entities;
using System.Threading.Tasks;

namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Validator for flow entities.
/// </summary>
public class FlowValidator : IValidator<FlowEntity>
{
    private readonly IRepository<ProcessingChainEntity> _processingChainRepository;
    private readonly IRepository<ImporterEntity> _importerRepository;
    private readonly IRepository<ExporterEntity> _exporterRepository;
    private readonly IRepository<ProcessorEntity> _processorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowValidator"/> class.
    /// </summary>
    /// <param name="processingChainRepository">The processing chain repository.</param>
    /// <param name="importerRepository">The importer repository.</param>
    /// <param name="exporterRepository">The exporter repository.</param>
    /// <param name="processorRepository">The processor repository.</param>
    public FlowValidator(
        IRepository<ProcessingChainEntity> processingChainRepository,
        IRepository<ImporterEntity> importerRepository,
        IRepository<ExporterEntity> exporterRepository,
        IRepository<ProcessorEntity> processorRepository)
    {
        _processingChainRepository = processingChainRepository;
        _importerRepository = importerRepository;
        _exporterRepository = exporterRepository;
        _processorRepository = processorRepository;
    }

    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(FlowEntity entity)
    {
        return ValidateAsync(entity).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Validates the specified entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateAsync(FlowEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.FlowName))
        {
            result.AddError("FlowName", "Flow name is required.");
        }

        if (entity.BranchConfigurations == null || entity.BranchConfigurations.Count == 0)
        {
            result.AddError("BranchConfigurations", "At least one branch configuration is required.");
            return result;
        }

        // Validate branch configurations
        foreach (var branchConfig in entity.BranchConfigurations)
        {
            if (string.IsNullOrWhiteSpace(branchConfig.Value.BranchId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].BranchId", "Branch ID is required.");
            }

            if (string.IsNullOrWhiteSpace(branchConfig.Value.ProcessingChainId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].ProcessingChainId", "Processing chain ID is required.");
            }
            else
            {
                // Validate that the processing chain exists
                var processingChain = await _processingChainRepository.GetByIdAsync(branchConfig.Value.ProcessingChainId);
                if (processingChain == null)
                {
                    result.AddError($"BranchConfigurations[{branchConfig.Key}].ProcessingChainId",
                        $"Processing chain with ID {branchConfig.Value.ProcessingChainId} not found.");
                }
            }
        }

        // If any branch configurations are invalid, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate execution mode
        if (!IsValidExecutionMode(entity.ExecutionMode))
        {
            result.AddError("ExecutionMode", $"Invalid execution mode: {entity.ExecutionMode}. Valid values are: Sequential, Parallel.");
        }

        // Validate retry policy
        if (entity.RetryPolicy != null)
        {
            if (entity.RetryPolicy.MaxRetries < 0)
            {
                result.AddError("RetryPolicy.MaxRetries", "Maximum retries cannot be negative.");
            }

            if (entity.RetryPolicy.RetryIntervalSeconds <= 0)
            {
                result.AddError("RetryPolicy.RetryIntervalSeconds", "Retry interval must be greater than zero.");
            }
        }

        return result;
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
        var result = new ValidationResult();

        // Get the entities
        var importer = await _importerRepository.GetByIdAsync(importerId);
        var processingChain = await _processingChainRepository.GetByIdAsync(processingChainId);
        var exporter = await _exporterRepository.GetByIdAsync(exporterId);

        // Validate that all entities exist
        if (importer == null)
        {
            result.AddError("ImporterId", $"Importer with ID {importerId} not found.");
        }

        if (processingChain == null)
        {
            result.AddError("ProcessingChainId", $"Processing chain with ID {processingChainId} not found.");
        }

        if (exporter == null)
        {
            result.AddError("ExporterId", $"Exporter with ID {exporterId} not found.");
        }

        // If any entities don't exist, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Get the first and last processors in the chain
        var firstProcessorId = processingChain.ProcessorIds.FirstOrDefault();
        var lastProcessorId = processingChain.ProcessorIds.LastOrDefault();

        if (string.IsNullOrEmpty(firstProcessorId) || string.IsNullOrEmpty(lastProcessorId))
        {
            result.AddError("ProcessingChainId", "Processing chain must have at least one processor.");
            return result;
        }

        var firstProcessor = await _processorRepository.GetByIdAsync(firstProcessorId);
        var lastProcessor = await _processorRepository.GetByIdAsync(lastProcessorId);

        // Validate importer-processor compatibility
        if (!AreSchemaCompatible(importer.OutputSchema, firstProcessor.InputSchema))
        {
            result.AddError("Compatibility",
                $"Output schema of importer {importer.Name} is not compatible with input schema of processor {firstProcessor.Name}.");
        }

        // Validate processor-exporter compatibility
        if (!AreSchemaCompatible(lastProcessor.OutputSchema, exporter.InputSchema))
        {
            result.AddError("Compatibility",
                $"Output schema of processor {lastProcessor.Name} is not compatible with input schema of exporter {exporter.Name}.");
        }

        return result;
    }

    private bool IsValidExecutionMode(string executionMode)
    {
        return executionMode == "Sequential" || executionMode == "Parallel";
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

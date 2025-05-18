using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Domain.Entities;
using System.Threading.Tasks;

namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Validator for processing chain entities.
/// </summary>
public class ProcessingChainValidator : IValidator<ProcessingChainEntity>
{
    private readonly IRepository<ProcessorEntity> _processorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingChainValidator"/> class.
    /// </summary>
    /// <param name="processorRepository">The processor repository.</param>
    public ProcessingChainValidator(IRepository<ProcessorEntity> processorRepository)
    {
        _processorRepository = processorRepository;
    }

    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(ProcessingChainEntity entity)
    {
        return ValidateAsync(entity).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Validates the specified entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateAsync(ProcessingChainEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            result.AddError("Name", "Name is required.");
        }

        if (entity.ProcessorIds == null || entity.ProcessorIds.Count == 0)
        {
            result.AddError("ProcessorIds", "At least one processor is required.");
            return result;
        }

        // Validate that all processors exist
        foreach (var processorId in entity.ProcessorIds)
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

        // Validate processor compatibility
        await ValidateProcessorCompatibilityAsync(entity, result);

        return result;
    }

    private async Task ValidateProcessorCompatibilityAsync(ProcessingChainEntity entity, ValidationResult result)
    {
        // For a chain with a single processor, no compatibility check is needed
        if (entity.ProcessorIds.Count == 1)
        {
            return;
        }

        // Check compatibility between adjacent processors in the chain
        for (int i = 0; i < entity.ProcessorIds.Count - 1; i++)
        {
            var sourceProcessorId = entity.ProcessorIds[i];
            var targetProcessorId = entity.ProcessorIds[i + 1];

            var sourceProcessor = await _processorRepository.GetByIdAsync(sourceProcessorId);
            var targetProcessor = await _processorRepository.GetByIdAsync(targetProcessorId);

            // Validate schema compatibility
            if (!AreSchemaCompatible(sourceProcessor.OutputSchema, targetProcessor.InputSchema))
            {
                result.AddError("ProcessorIds",
                    $"Output schema of processor {sourceProcessor.Name} is not compatible with input schema of processor {targetProcessor.Name}.");
            }
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

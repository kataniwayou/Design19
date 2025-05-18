using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.Common.Utilities;

namespace FlowOrchestrator.ProcessorEntityManager.Validators;

/// <summary>
/// Validator for processor entities.
/// </summary>
public class ProcessorValidator : IValidator<ProcessorEntity>
{
    /// <summary>
    /// Validates the specified processor entity.
    /// </summary>
    /// <param name="obj">The processor entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(ProcessorEntity obj)
    {
        var result = new ValidationResult();

        // Validate required properties
        if (StringUtilities.IsNullOrWhiteSpace(obj.Name))
        {
            result.AddError(nameof(obj.Name), "Name is required.");
        }

        if (StringUtilities.IsNullOrWhiteSpace(obj.ProcessorType))
        {
            result.AddError(nameof(obj.ProcessorType), "ProcessorType is required.");
        }

        if (StringUtilities.IsNullOrWhiteSpace(obj.Version))
        {
            result.AddError(nameof(obj.Version), "Version is required.");
        }

        // Validate input schema
        if (obj.InputSchema == null)
        {
            result.AddError(nameof(obj.InputSchema), "InputSchema is required.");
        }
        else if (obj.InputSchema.Fields == null || obj.InputSchema.Fields.Count == 0)
        {
            result.AddError(nameof(obj.InputSchema), "InputSchema must have at least one field.");
        }

        // Validate output schema
        if (obj.OutputSchema == null)
        {
            result.AddError(nameof(obj.OutputSchema), "OutputSchema is required.");
        }
        else if (obj.OutputSchema.Fields == null || obj.OutputSchema.Fields.Count == 0)
        {
            result.AddError(nameof(obj.OutputSchema), "OutputSchema must have at least one field.");
        }

        // Validate capabilities
        if (obj.Capabilities == null || obj.Capabilities.Count == 0)
        {
            result.AddError(nameof(obj.Capabilities), "At least one capability is required.");
        }

        return result;
    }
}

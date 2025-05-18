using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;
using FlowOrchestrator.Common.Utilities;

namespace FlowOrchestrator.ImporterEntityManager.Validators;

/// <summary>
/// Validator for importer entities.
/// </summary>
public class ImporterValidator : IValidator<ImporterEntity>
{
    /// <summary>
    /// Validates the specified importer entity.
    /// </summary>
    /// <param name="obj">The importer entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(ImporterEntity obj)
    {
        var result = new ValidationResult();

        // Validate required properties
        if (StringUtilities.IsNullOrWhiteSpace(obj.Name))
        {
            result.AddError(nameof(obj.Name), "Name is required.");
        }

        if (StringUtilities.IsNullOrWhiteSpace(obj.ImporterType))
        {
            result.AddError(nameof(obj.ImporterType), "ImporterType is required.");
        }

        if (StringUtilities.IsNullOrWhiteSpace(obj.Protocol))
        {
            result.AddError(nameof(obj.Protocol), "Protocol is required.");
        }

        if (StringUtilities.IsNullOrWhiteSpace(obj.Version))
        {
            result.AddError(nameof(obj.Version), "Version is required.");
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

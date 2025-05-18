using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;

namespace FlowOrchestrator.ExporterEntityManager.Validators;

/// <summary>
/// Validator for exporter entities.
/// </summary>
public class ExporterValidator : IValidator<ExporterEntity>
{
    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(ExporterEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            result.AddError("Name", "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.ExporterType))
        {
            result.AddError("ExporterType", "Exporter type is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.Protocol))
        {
            result.AddError("Protocol", "Protocol is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.Version))
        {
            result.AddError("Version", "Version is required.");
        }

        if (entity.InputSchema == null || entity.InputSchema.Fields == null || entity.InputSchema.Fields.Count == 0)
        {
            result.AddError("InputSchema", "Input schema is required and must contain at least one field.");
        }

        return result;
    }
}

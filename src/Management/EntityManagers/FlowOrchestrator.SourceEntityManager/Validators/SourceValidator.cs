using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;

namespace FlowOrchestrator.SourceEntityManager.Validators;

/// <summary>
/// Validator for source entities.
/// </summary>
public class SourceValidator : IValidator<SourceEntity>
{
    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(SourceEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            result.AddError("Name", "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.SourceType))
        {
            result.AddError("SourceType", "Source type is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.Address))
        {
            result.AddError("Address", "Address is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.Protocol))
        {
            result.AddError("Protocol", "Protocol is required.");
        }

        return result;
    }
}

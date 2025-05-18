using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Domain.Validation;

namespace FlowOrchestrator.DestinationEntityManager.Validators;

/// <summary>
/// Validator for destination entities.
/// </summary>
public class DestinationValidator : IValidator<DestinationEntity>
{
    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(DestinationEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            result.AddError("Name", "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.DestinationType))
        {
            result.AddError("DestinationType", "Destination type is required.");
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

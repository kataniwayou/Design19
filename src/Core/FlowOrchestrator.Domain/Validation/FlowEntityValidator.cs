using FlowOrchestrator.Common.Utilities;
using FlowOrchestrator.Domain.Entities;

namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Validator for flow entities.
/// </summary>
public class FlowEntityValidator : IValidator<FlowEntity>
{
    /// <summary>
    /// Validates the specified flow entity.
    /// </summary>
    /// <param name="obj">The flow entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(FlowEntity obj)
    {
        var result = new ValidationResult();

        // Validate required properties
        if (StringUtilities.IsNullOrWhiteSpace(obj.Name))
        {
            result.AddError(nameof(obj.Name), "Name is required.");
        }

        if (StringUtilities.IsNullOrWhiteSpace(obj.ImporterId))
        {
            result.AddError(nameof(obj.ImporterId), "ImporterId is required.");
        }

        if (obj.ProcessingChainIds.Count == 0)
        {
            result.AddError(nameof(obj.ProcessingChainIds), "At least one processing chain is required.");
        }

        if (obj.ExporterIds.Count == 0)
        {
            result.AddError(nameof(obj.ExporterIds), "At least one exporter is required.");
        }

        // Validate branch configurations
        foreach (var branchConfig in obj.BranchConfigurations)
        {
            if (StringUtilities.IsNullOrWhiteSpace(branchConfig.Key))
            {
                result.AddError("BranchConfigurations", "Branch key cannot be empty.");
            }

            if (StringUtilities.IsNullOrWhiteSpace(branchConfig.Value.BranchId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].BranchId", "Branch ID is required.");
            }

            if (StringUtilities.IsNullOrWhiteSpace(branchConfig.Value.ProcessingChainId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].ProcessingChainId", "Processing chain ID is required.");
            }

            if (StringUtilities.IsNullOrWhiteSpace(branchConfig.Value.ExporterId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].ExporterId", "Exporter ID is required.");
            }

            if (!obj.ProcessingChainIds.Contains(branchConfig.Value.ProcessingChainId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].ProcessingChainId", $"Processing chain ID '{branchConfig.Value.ProcessingChainId}' is not in the list of processing chain IDs.");
            }

            if (!obj.ExporterIds.Contains(branchConfig.Value.ExporterId))
            {
                result.AddError($"BranchConfigurations[{branchConfig.Key}].ExporterId", $"Exporter ID '{branchConfig.Value.ExporterId}' is not in the list of exporter IDs.");
            }
        }

        return result;
    }
}

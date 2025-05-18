using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Domain.Entities;
using System.Threading.Tasks;

namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Validator for destination assignment entities.
/// </summary>
public class DestinationAssignmentValidator : IValidator<DestinationAssignmentEntity>
{
    private readonly IRepository<DestinationEntity> _destinationRepository;
    private readonly IRepository<ExporterEntity> _exporterRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DestinationAssignmentValidator"/> class.
    /// </summary>
    /// <param name="destinationRepository">The destination repository.</param>
    /// <param name="exporterRepository">The exporter repository.</param>
    public DestinationAssignmentValidator(
        IRepository<DestinationEntity> destinationRepository,
        IRepository<ExporterEntity> exporterRepository)
    {
        _destinationRepository = destinationRepository;
        _exporterRepository = exporterRepository;
    }

    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(DestinationAssignmentEntity entity)
    {
        return ValidateAsync(entity).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Validates the specified entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateAsync(DestinationAssignmentEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.DestinationId))
        {
            result.AddError("DestinationId", "Destination ID is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.ExporterId))
        {
            result.AddError("ExporterId", "Exporter ID is required.");
        }

        // If basic validation fails, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate that destination and exporter exist
        var destination = await _destinationRepository.GetByIdAsync(entity.DestinationId);
        var exporter = await _exporterRepository.GetByIdAsync(entity.ExporterId);

        if (destination == null)
        {
            result.AddError("DestinationId", $"Destination with ID {entity.DestinationId} not found.");
        }

        if (exporter == null)
        {
            result.AddError("ExporterId", $"Exporter with ID {entity.ExporterId} not found.");
        }

        // If destination or exporter don't exist, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate destination-exporter compatibility
        if (!IsDestinationCompatibleWithExporter(destination, exporter))
        {
            result.AddError("Compatibility",
                $"Destination {destination.Name} is not compatible with exporter {exporter.Name}.");
        }

        // Validate configuration parameters
        if (entity.Configuration != null)
        {
            foreach (var param in exporter.ConfigurationParameters)
            {
                if (param.Value.IsRequired && !entity.Configuration.ContainsKey(param.Key))
                {
                    result.AddError($"Configuration.{param.Key}",
                        $"Required configuration parameter {param.Key} is missing.");
                }
            }
        }
        else if (exporter.ConfigurationParameters.Any(p => p.Value.IsRequired))
        {
            result.AddError("Configuration", "Configuration is required for this exporter.");
        }

        return result;
    }

    private bool IsDestinationCompatibleWithExporter(DestinationEntity destination, ExporterEntity exporter)
    {
        // Check if the exporter supports the destination's protocol
        if (!string.IsNullOrEmpty(destination.DataFormat) &&
            !exporter.Capabilities.Contains(destination.DataFormat))
        {
            return false;
        }

        // Check if the exporter supports any of the destination's protocols
        if (destination.SupportedProtocols.Count > 0 &&
            !destination.SupportedProtocols.Contains(exporter.Protocol))
        {
            return false;
        }

        return true;
    }
}

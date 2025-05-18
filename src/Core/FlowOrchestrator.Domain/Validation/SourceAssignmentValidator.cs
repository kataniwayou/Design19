using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Domain.Entities;
using System.Threading.Tasks;

namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Validator for source assignment entities.
/// </summary>
public class SourceAssignmentValidator : IValidator<SourceAssignmentEntity>
{
    private readonly IRepository<SourceEntity> _sourceRepository;
    private readonly IRepository<ImporterEntity> _importerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceAssignmentValidator"/> class.
    /// </summary>
    /// <param name="sourceRepository">The source repository.</param>
    /// <param name="importerRepository">The importer repository.</param>
    public SourceAssignmentValidator(
        IRepository<SourceEntity> sourceRepository,
        IRepository<ImporterEntity> importerRepository)
    {
        _sourceRepository = sourceRepository;
        _importerRepository = importerRepository;
    }

    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(SourceAssignmentEntity entity)
    {
        return ValidateAsync(entity).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Validates the specified entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateAsync(SourceAssignmentEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.SourceId))
        {
            result.AddError("SourceId", "Source ID is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.ImporterId))
        {
            result.AddError("ImporterId", "Importer ID is required.");
        }

        // If basic validation fails, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate that source and importer exist
        var source = await _sourceRepository.GetByIdAsync(entity.SourceId);
        var importer = await _importerRepository.GetByIdAsync(entity.ImporterId);

        if (source == null)
        {
            result.AddError("SourceId", $"Source with ID {entity.SourceId} not found.");
        }

        if (importer == null)
        {
            result.AddError("ImporterId", $"Importer with ID {entity.ImporterId} not found.");
        }

        // If source or importer don't exist, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate source-importer compatibility
        if (!IsSourceCompatibleWithImporter(source, importer))
        {
            result.AddError("Compatibility",
                $"Source {source.Name} is not compatible with importer {importer.Name}.");
        }

        // Validate configuration parameters
        if (entity.Configuration != null)
        {
            foreach (var param in importer.ConfigurationParameters)
            {
                if (param.Value.IsRequired && !entity.Configuration.ContainsKey(param.Key))
                {
                    result.AddError($"Configuration.{param.Key}",
                        $"Required configuration parameter {param.Key} is missing.");
                }
            }
        }
        else if (importer.ConfigurationParameters.Any(p => p.Value.IsRequired))
        {
            result.AddError("Configuration", "Configuration is required for this importer.");
        }

        return result;
    }

    private bool IsSourceCompatibleWithImporter(SourceEntity source, ImporterEntity importer)
    {
        // Check if the importer supports the source's protocol
        if (!string.IsNullOrEmpty(source.DataFormat) &&
            !importer.Capabilities.Contains(source.DataFormat))
        {
            return false;
        }

        // Check if the importer supports any of the source's protocols
        if (source.SupportedProtocols.Count > 0 &&
            !source.SupportedProtocols.Contains(importer.Protocol))
        {
            return false;
        }

        return true;
    }
}

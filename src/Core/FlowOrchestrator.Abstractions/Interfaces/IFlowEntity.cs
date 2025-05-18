namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a flow entity in the FlowOrchestrator system.
/// A flow entity represents a complete end-to-end data pipeline connecting sources to destinations.
/// </summary>
public interface IFlowEntity : IEntity
{
    /// <summary>
    /// Gets the identifier of the importer service.
    /// </summary>
    string ImporterId { get; }

    /// <summary>
    /// Gets the identifiers of the processing chains.
    /// </summary>
    IReadOnlyList<string> ProcessingChainIds { get; }

    /// <summary>
    /// Gets the identifiers of the exporter services.
    /// </summary>
    IReadOnlyList<string> ExporterIds { get; }

    /// <summary>
    /// Gets the flow branch configuration.
    /// </summary>
    IReadOnlyDictionary<string, FlowBranchConfiguration> BranchConfigurations { get; }
}

/// <summary>
/// Represents the configuration of a flow branch.
/// </summary>
public class FlowBranchConfiguration
{
    /// <summary>
    /// Gets or sets the identifier of the branch.
    /// </summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the processing chain.
    /// </summary>
    public string ProcessingChainId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the exporter service.
    /// </summary>
    public string ExporterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the branch is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the branch execution priority.
    /// </summary>
    public int Priority { get; set; } = 0;
}

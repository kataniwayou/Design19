namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a processing chain entity in the FlowOrchestrator system.
/// A processing chain entity represents a directed acyclic graph of processor services that defines data transformation logic.
/// </summary>
public interface IProcessingChainEntity : IEntity
{
    /// <summary>
    /// Gets the identifiers of the processor services.
    /// </summary>
    IReadOnlyList<string> ProcessorIds { get; }

    /// <summary>
    /// Gets the processing chain structure.
    /// </summary>
    ProcessingChainStructure ChainStructure { get; }
}

/// <summary>
/// Represents the structure of a processing chain.
/// </summary>
public class ProcessingChainStructure
{
    /// <summary>
    /// Gets or sets the entry points of the processing chain.
    /// </summary>
    public List<string> EntryPoints { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the connections between processors.
    /// </summary>
    public Dictionary<string, List<string>> Connections { get; set; } = new Dictionary<string, List<string>>();

    /// <summary>
    /// Gets or sets the exit points of the processing chain.
    /// </summary>
    public List<string> ExitPoints { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the processor configurations.
    /// </summary>
    public Dictionary<string, ProcessorConfiguration> ProcessorConfigurations { get; set; } = new Dictionary<string, ProcessorConfiguration>();
}

/// <summary>
/// Represents the configuration of a processor.
/// </summary>
public class ProcessorConfiguration
{
    /// <summary>
    /// Gets or sets the identifier of the processor.
    /// </summary>
    public string ProcessorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch path of the processor.
    /// </summary>
    public string BranchPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position of the processor within the branch.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the processor is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the processor-specific configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
}

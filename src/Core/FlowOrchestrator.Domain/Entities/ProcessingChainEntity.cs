using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a processing chain entity in the FlowOrchestrator system.
/// A processing chain entity represents a directed acyclic graph of processor services that defines data transformation logic.
/// </summary>
public class ProcessingChainEntity : AbstractEntity, IProcessingChainEntity
{
    /// <summary>
    /// Gets or sets the identifiers of the processor services.
    /// </summary>
    public List<string> ProcessorIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the processing chain structure.
    /// </summary>
    public ProcessingChainStructure ChainStructure { get; set; } = new ProcessingChainStructure();

    /// <summary>
    /// Gets or sets the chain type.
    /// </summary>
    public string ChainType { get; set; } = string.Empty;

    /// <summary>
    /// Gets the identifiers of the processor services.
    /// </summary>
    IReadOnlyList<string> IProcessingChainEntity.ProcessorIds => ProcessorIds.AsReadOnly();
}

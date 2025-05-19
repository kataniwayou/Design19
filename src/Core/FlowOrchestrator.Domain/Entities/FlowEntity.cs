using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a flow entity in the FlowOrchestrator system.
/// A flow entity represents a complete end-to-end data pipeline connecting sources to destinations.
/// </summary>
public class FlowEntity : AbstractEntity, IFlowEntity
{
    /// <summary>
    /// Gets or sets the flow name.
    /// </summary>
    public string FlowName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the importer service.
    /// </summary>
    public string ImporterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifiers of the processing chains.
    /// </summary>
    public List<string> ProcessingChainIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the identifier of the processing chain.
    /// </summary>
    public string ProcessingChainId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifiers of the exporter services.
    /// </summary>
    public List<string> ExporterIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the flow branch configuration.
    /// </summary>
    public Dictionary<string, FlowBranchConfiguration> BranchConfigurations { get; set; } = new Dictionary<string, FlowBranchConfiguration>();

    /// <summary>
    /// Gets or sets the branch identifiers.
    /// </summary>
    public List<string> BranchIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the execution mode.
    /// </summary>
    public string ExecutionMode { get; set; } = "Sequential";

    /// <summary>
    /// Gets or sets the retry policy.
    /// </summary>
    public RetryPolicy? RetryPolicy { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Gets or sets the source assignment identifier.
    /// </summary>
    public string SourceAssignmentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination assignment identifier.
    /// </summary>
    public string DestinationAssignmentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination assignment identifiers.
    /// </summary>
    public List<string> DestinationAssignmentIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the flow is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the created at timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the updated at timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the created by user.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated by user.
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets the identifiers of the processing chains.
    /// </summary>
    IReadOnlyList<string> IFlowEntity.ProcessingChainIds => ProcessingChainIds.AsReadOnly();

    /// <summary>
    /// Gets the identifiers of the exporter services.
    /// </summary>
    IReadOnlyList<string> IFlowEntity.ExporterIds => ExporterIds.AsReadOnly();

    /// <summary>
    /// Gets the flow branch configuration.
    /// </summary>
    IReadOnlyDictionary<string, FlowBranchConfiguration> IFlowEntity.BranchConfigurations => BranchConfigurations;
}

/// <summary>
/// Represents a retry policy for a flow.
/// </summary>
public class RetryPolicy
{
    /// <summary>
    /// Gets or sets the maximum number of retries.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry interval in seconds.
    /// </summary>
    public int RetryIntervalSeconds { get; set; } = 60;
}

using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a destination assignment entity in the FlowOrchestrator system.
/// A destination assignment entity represents the assignment of a destination to an exporter.
/// </summary>
public class DestinationAssignmentEntity : AbstractEntity, IDestinationAssignmentEntity
{
    /// <summary>
    /// Gets or sets the identifier of the destination.
    /// </summary>
    public string DestinationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the exporter.
    /// </summary>
    public string ExporterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the exporter type.
    /// </summary>
    public string ExporterType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assignment configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the configuration parameters.
    /// </summary>
    public Dictionary<string, string> ConfigurationParameters { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets the assignment configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> IDestinationAssignmentEntity.Configuration => Configuration;
}

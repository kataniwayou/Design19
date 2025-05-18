using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a source assignment entity in the FlowOrchestrator system.
/// A source assignment entity represents the assignment of a source to an importer.
/// </summary>
public class SourceAssignmentEntity : AbstractEntity, ISourceAssignmentEntity
{
    /// <summary>
    /// Gets or sets the identifier of the source.
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the importer.
    /// </summary>
    public string ImporterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assignment configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the assignment configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> ISourceAssignmentEntity.Configuration => Configuration;
}

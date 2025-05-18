namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a source assignment entity in the FlowOrchestrator system.
/// A source assignment entity represents the assignment of a source to an importer.
/// </summary>
public interface ISourceAssignmentEntity : IEntity
{
    /// <summary>
    /// Gets the identifier of the source.
    /// </summary>
    string SourceId { get; }

    /// <summary>
    /// Gets the identifier of the importer.
    /// </summary>
    string ImporterId { get; }

    /// <summary>
    /// Gets the assignment configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> Configuration { get; }
}

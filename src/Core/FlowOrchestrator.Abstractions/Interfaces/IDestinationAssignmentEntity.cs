namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a destination assignment entity in the FlowOrchestrator system.
/// A destination assignment entity represents the assignment of a destination to an exporter.
/// </summary>
public interface IDestinationAssignmentEntity : IEntity
{
    /// <summary>
    /// Gets the identifier of the destination.
    /// </summary>
    string DestinationId { get; }

    /// <summary>
    /// Gets the identifier of the exporter.
    /// </summary>
    string ExporterId { get; }

    /// <summary>
    /// Gets the assignment configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> Configuration { get; }
}

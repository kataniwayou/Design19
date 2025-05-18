namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a source entity in the FlowOrchestrator system.
/// A source entity represents a data source location and access protocol.
/// </summary>
public interface ISourceEntity : IEntity
{
    /// <summary>
    /// Gets the source type.
    /// </summary>
    string SourceType { get; }

    /// <summary>
    /// Gets the source address.
    /// </summary>
    string Address { get; }

    /// <summary>
    /// Gets the connection protocol.
    /// </summary>
    string Protocol { get; }

    /// <summary>
    /// Gets the source configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> Configuration { get; }
}

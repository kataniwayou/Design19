namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a destination entity in the FlowOrchestrator system.
/// A destination entity represents a data destination location and access protocol.
/// </summary>
public interface IDestinationEntity : IEntity
{
    /// <summary>
    /// Gets the destination type.
    /// </summary>
    string DestinationType { get; }

    /// <summary>
    /// Gets the destination address.
    /// </summary>
    string Address { get; }

    /// <summary>
    /// Gets the connection protocol.
    /// </summary>
    string Protocol { get; }

    /// <summary>
    /// Gets the destination configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> Configuration { get; }
}

using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a destination entity in the FlowOrchestrator system.
/// A destination entity represents a data destination location and access protocol.
/// </summary>
public class DestinationEntity : AbstractEntity, IDestinationEntity
{
    /// <summary>
    /// Gets or sets the destination type.
    /// </summary>
    public string DestinationType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the connection protocol.
    /// </summary>
    public string Protocol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data format.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the supported protocols.
    /// </summary>
    public List<string> SupportedProtocols { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the destination configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the destination configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> IDestinationEntity.Configuration => Configuration;
}

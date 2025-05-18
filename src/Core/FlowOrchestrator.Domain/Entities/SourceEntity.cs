using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a source entity in the FlowOrchestrator system.
/// A source entity represents a data source location and access protocol.
/// </summary>
public class SourceEntity : AbstractEntity, ISourceEntity
{
    /// <summary>
    /// Gets or sets the source type.
    /// </summary>
    public string SourceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source address.
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
    /// Gets or sets the source configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the source configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> ISourceEntity.Configuration => Configuration;
}

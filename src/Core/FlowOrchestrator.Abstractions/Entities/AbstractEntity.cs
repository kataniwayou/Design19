using System.Text.Json.Serialization;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Abstractions.Entities;

/// <summary>
/// Base class for all entities in the FlowOrchestrator system.
/// </summary>
public abstract class AbstractEntity : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the name of the entity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the entity.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the entity.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    public DateTime LastModifiedTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the description of the current version.
    /// </summary>
    public string VersionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the previous version of the entity.
    /// </summary>
    public string? PreviousVersionId { get; set; }

    /// <summary>
    /// Gets or sets the status of the entity version.
    /// </summary>
    public Interfaces.VersionStatus VersionStatus { get; set; } = Interfaces.VersionStatus.Active;

    /// <summary>
    /// Gets or sets the tags associated with the entity.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets the tags associated with the entity.
    /// </summary>
    IReadOnlyList<string> IEntity.Tags => Tags.AsReadOnly();

    /// <summary>
    /// Gets or sets the metadata associated with the entity.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the metadata associated with the entity.
    /// </summary>
    IReadOnlyDictionary<string, object> IEntity.Metadata => Metadata;

}

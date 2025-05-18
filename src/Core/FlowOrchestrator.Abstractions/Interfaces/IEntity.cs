namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for an entity in the FlowOrchestrator system.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the entity.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the entity.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of the entity.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the timestamp when the entity was created.
    /// </summary>
    DateTime CreatedTimestamp { get; }

    /// <summary>
    /// Gets the timestamp when the entity was last modified.
    /// </summary>
    DateTime LastModifiedTimestamp { get; }

    /// <summary>
    /// Gets the description of the current version.
    /// </summary>
    string VersionDescription { get; }

    /// <summary>
    /// Gets the identifier of the previous version of the entity.
    /// </summary>
    string? PreviousVersionId { get; }

    /// <summary>
    /// Gets the status of the entity version.
    /// </summary>
    VersionStatus VersionStatus { get; }

    /// <summary>
    /// Gets the tags associated with the entity.
    /// </summary>
    IReadOnlyList<string> Tags { get; }

    /// <summary>
    /// Gets the metadata associated with the entity.
    /// </summary>
    IReadOnlyDictionary<string, object> Metadata { get; }
}

/// <summary>
/// Defines the status of an entity version.
/// </summary>
public enum VersionStatus
{
    /// <summary>
    /// The entity version is active and can be used.
    /// </summary>
    Active,

    /// <summary>
    /// The entity version is deprecated and should not be used for new flows.
    /// </summary>
    Deprecated,

    /// <summary>
    /// The entity version is archived and cannot be used.
    /// </summary>
    Archived
}

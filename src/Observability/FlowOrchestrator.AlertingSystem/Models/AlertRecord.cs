using FlowOrchestrator.ObservabilityBase.Monitoring;
using MongoDB.Bson.Serialization.Attributes;

namespace FlowOrchestrator.AlertingSystem;

/// <summary>
/// Represents an alert record.
/// </summary>
public class AlertRecord
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [BsonId]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity.
    /// </summary>
    public AlertSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the start timestamp.
    /// </summary>
    public DateTime StartTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime LastUpdateTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the end timestamp.
    /// </summary>
    public DateTime? EndTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public List<string>? Tags { get; set; }
}

using FlowOrchestrator.ObservabilityBase.Monitoring;
using MongoDB.Bson.Serialization.Attributes;

namespace FlowOrchestrator.MonitoringFramework;

/// <summary>
/// Represents a health check record.
/// </summary>
public class HealthCheckRecord
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
    /// Gets or sets the component identifier.
    /// </summary>
    public string ComponentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check name.
    /// </summary>
    public string CheckName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public HealthCheckStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }
}

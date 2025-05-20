using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.ObservabilityBase.Monitoring;

namespace FlowOrchestrator.MonitoringFramework.Messages;

/// <summary>
/// Represents a health check message.
/// </summary>
public class HealthCheckMessage : IMessage
{
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

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

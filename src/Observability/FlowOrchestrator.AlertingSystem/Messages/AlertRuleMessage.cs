using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.ObservabilityBase.Monitoring;

namespace FlowOrchestrator.AlertingSystem.Messages;

/// <summary>
/// Represents an alert rule message.
/// </summary>
public class AlertRuleMessage : IMessage
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
    /// Gets or sets the tags.
    /// </summary>
    public IEnumerable<string>? Tags { get; set; }
}

using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.AnalyticsEngine.Messages;

/// <summary>
/// Represents a report request message.
/// </summary>
public class ReportRequestMessage : IMessage
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
    /// Gets or sets the report type.
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }
}

using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.AnalyticsEngine.Messages;

/// <summary>
/// Represents a report response message.
/// </summary>
public class ReportResponseMessage : IMessage
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
    /// Gets or sets the report identifier.
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the report type.
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}

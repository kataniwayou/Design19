using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents a status message for an exporter.
/// </summary>
public class ExporterStatusMessage : EventBase
{
    /// <summary>
    /// Gets the exporter identifier.
    /// </summary>
    public string ExporterId { get; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExporterStatusMessage"/> class.
    /// </summary>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ExporterStatusMessage(
        string exporterId,
        string status,
        string message,
        string correlationId)
        : base(correlationId)
    {
        ExporterId = exporterId;
        Status = status;
        Message = message;
    }
}

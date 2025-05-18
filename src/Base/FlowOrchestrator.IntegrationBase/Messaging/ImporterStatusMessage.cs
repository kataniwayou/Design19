using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents a status message for an importer.
/// </summary>
public class ImporterStatusMessage : EventBase
{
    /// <summary>
    /// Gets the importer identifier.
    /// </summary>
    public string ImporterId { get; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImporterStatusMessage"/> class.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ImporterStatusMessage(
        string importerId,
        string status,
        string message,
        string correlationId)
        : base(correlationId)
    {
        ImporterId = importerId;
        Status = status;
        Message = message;
    }
}

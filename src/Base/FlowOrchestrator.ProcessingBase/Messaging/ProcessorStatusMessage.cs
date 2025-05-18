using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.ProcessingBase.Messaging;

/// <summary>
/// Represents a status message for a processor.
/// </summary>
public class ProcessorStatusMessage : EventBase
{
    /// <summary>
    /// Gets the processor identifier.
    /// </summary>
    public string ProcessorId { get; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorStatusMessage"/> class.
    /// </summary>
    /// <param name="processorId">The processor identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ProcessorStatusMessage(
        string processorId,
        string status,
        string message,
        string correlationId)
        : base(correlationId)
    {
        ProcessorId = processorId;
        Status = status;
        Message = message;
    }
}

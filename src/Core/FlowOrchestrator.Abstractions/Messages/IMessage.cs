namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Base interface for all messages in the FlowOrchestrator system.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Gets the unique identifier of the message.
    /// </summary>
    string MessageId { get; }

    /// <summary>
    /// Gets the timestamp when the message was created.
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    /// Gets the correlation identifier for tracking related messages.
    /// </summary>
    string CorrelationId { get; }
}

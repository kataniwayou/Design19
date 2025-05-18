namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Base class for all messages in the FlowOrchestrator system.
/// </summary>
public abstract class MessageBase : IMessage
{
    /// <summary>
    /// Gets the unique identifier of the message.
    /// </summary>
    public string MessageId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the correlation identifier for tracking related messages.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBase"/> class.
    /// </summary>
    /// <param name="correlationId">The correlation identifier for tracking related messages.</param>
    protected MessageBase(string correlationId)
    {
        CorrelationId = string.IsNullOrEmpty(correlationId) ? Guid.NewGuid().ToString() : correlationId;
    }
}

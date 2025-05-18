namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Base class for all event messages in the FlowOrchestrator system.
/// </summary>
public abstract class EventBase : MessageBase, IEvent
{
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    public string EventType => GetType().Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBase"/> class.
    /// </summary>
    /// <param name="correlationId">The correlation identifier for tracking related messages.</param>
    protected EventBase(string correlationId) : base(correlationId)
    {
    }
}

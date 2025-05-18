namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Interface for event messages in the FlowOrchestrator system.
/// Events are messages that notify about something that has happened.
/// </summary>
public interface IEvent : IMessage
{
    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    string EventType { get; }
}

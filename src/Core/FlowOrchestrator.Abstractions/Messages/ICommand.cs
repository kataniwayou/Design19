namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Interface for command messages in the FlowOrchestrator system.
/// Commands are messages that request an action to be performed.
/// </summary>
public interface ICommand : IMessage
{
    /// <summary>
    /// Gets the type of the command.
    /// </summary>
    string CommandType { get; }
}

namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Base class for all command messages in the FlowOrchestrator system.
/// </summary>
public abstract class CommandBase : MessageBase, ICommand
{
    /// <summary>
    /// Gets the type of the command.
    /// </summary>
    public string CommandType => GetType().Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase"/> class.
    /// </summary>
    /// <param name="correlationId">The correlation identifier for tracking related messages.</param>
    protected CommandBase(string correlationId) : base(correlationId)
    {
    }
}

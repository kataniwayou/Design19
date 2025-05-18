using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.Infrastructure.Common.Messaging;

/// <summary>
/// Defines the contract for a message bus.
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publishes a message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class, IMessage;

    /// <summary>
    /// Sends a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand;
}

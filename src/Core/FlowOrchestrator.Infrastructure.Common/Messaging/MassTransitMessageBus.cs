using FlowOrchestrator.Abstractions.Messages;
using MassTransit;

namespace FlowOrchestrator.Infrastructure.Common.Messaging;

/// <summary>
/// MassTransit implementation of the message bus.
/// </summary>
public class MassTransitMessageBus : IMessageBus
{
    private readonly IBus _bus;

    /// <summary>
    /// Initializes a new instance of the <see cref="MassTransitMessageBus"/> class.
    /// </summary>
    /// <param name="bus">The MassTransit bus.</param>
    public MassTransitMessageBus(IBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Publishes a message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class, IMessage
    {
        await _bus.Publish(message, cancellationToken);
    }

    /// <summary>
    /// Sends a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{typeof(TCommand).Name}"));
        await endpoint.Send(command, cancellationToken);
    }
}

using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using MassTransit;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.MassTransit.Consumers;

/// <summary>
/// Abstract base consumer for status messages.
/// </summary>
/// <typeparam name="TMessage">The type of message.</typeparam>
public abstract class AbstractStatusConsumer<TMessage> : IConsumer<TMessage>
    where TMessage : class, IMessage
{
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractStatusConsumer{TMessage}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractStatusConsumer(
        ILogger logger,
        ITelemetryProvider telemetryProvider)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Consumes the message.
    /// </summary>
    /// <param name="context">The consume context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<TMessage> context)
    {
        var message = context.Message;
        var messageType = message.GetType().Name;

        using var span = _telemetryProvider.CreateSpan($"Consumer.{messageType}");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("correlation.id", message.CorrelationId);

        try
        {
            _logger.Info($"Received {messageType} message with ID: {message.MessageId}");

            await ProcessMessageAsync(message);

            _logger.Info($"Processed {messageType} message with ID: {message.MessageId}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process {messageType} message with ID: {message.MessageId}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Processes the message.
    /// </summary>
    /// <param name="message">The message to process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task ProcessMessageAsync(TMessage message);
}

using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using MassTransit;

namespace FlowOrchestrator.StatisticsService.Consumers;

/// <summary>
/// Consumer for statistics messages.
/// </summary>
public class StatisticsMessageConsumer : IConsumer<StatisticsMessage>
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly Services.StatisticsService _statisticsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsMessageConsumer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="statisticsService">The statistics service.</param>
    public StatisticsMessageConsumer(
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        Services.StatisticsService statisticsService)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Consumes the message.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<StatisticsMessage> context)
    {
        var message = context.Message;
        using var span = _telemetryProvider.CreateSpan("StatisticsMessageConsumer.Consume");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("statistics.category", message.Category);
        span.SetAttribute("statistics.name", message.Name);
        span.SetAttribute("statistics.type", message.Type.ToString());

        try
        {
            _logger.Info($"Received statistics message: {message.Category}.{message.Name} ({message.Type})");

            switch (message.Type)
            {
                case StatisticsType.Counter:
                    await _statisticsService.RecordCounterAsync(
                        message.Category,
                        message.Name,
                        Convert.ToInt64(message.Value),
                        message.Attributes);
                    break;

                case StatisticsType.Gauge:
                    await _statisticsService.RecordGaugeAsync(
                        message.Category,
                        message.Name,
                        message.Value,
                        message.Attributes);
                    break;

                case StatisticsType.Histogram:
                    await _statisticsService.RecordHistogramAsync(
                        message.Category,
                        message.Name,
                        message.Value,
                        message.Attributes);
                    break;

                default:
                    _logger.Info($"Unknown statistics type: {message.Type}");
                    break;
            }

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process statistics message: {message.Category}.{message.Name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

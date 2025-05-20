using FlowOrchestrator.AlertingSystem.Messages;
using FlowOrchestrator.AlertingSystem.Services;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using MassTransit;

namespace FlowOrchestrator.AlertingSystem.Consumers;

/// <summary>
/// Consumer for alert messages.
/// </summary>
public class AlertMessageConsumer : IConsumer<AlertRuleMessage>
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly AlertingService _alertingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertMessageConsumer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="alertingService">The alerting service.</param>
    public AlertMessageConsumer(
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        AlertingService alertingService)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _alertingService = alertingService;
    }

    /// <summary>
    /// Consumes the message.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<AlertRuleMessage> context)
    {
        var message = context.Message;
        using var span = _telemetryProvider.CreateSpan("AlertMessageConsumer.Consume");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("alert.rule.name", message.Name);
        span.SetAttribute("alert.severity", message.Severity.ToString());

        try
        {
            _logger.Info($"Received alert rule message: {message.Name} ({message.Severity})");

            // Register the alert rule
            await _alertingService.RegisterAlertRuleAsync(
                message.Name,
                async () =>
                {
                    // This is a placeholder for the actual condition
                    // In a real implementation, this would be based on the message content
                    return false;
                },
                message.Severity,
                message.Description,
                message.Tags);

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process alert rule message: {message.Name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

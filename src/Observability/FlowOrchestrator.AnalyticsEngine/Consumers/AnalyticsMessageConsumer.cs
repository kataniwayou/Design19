using FlowOrchestrator.AnalyticsEngine.Messages;
using FlowOrchestrator.AnalyticsEngine.Services;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using MassTransit;

namespace FlowOrchestrator.AnalyticsEngine.Consumers;

/// <summary>
/// Consumer for analytics messages.
/// </summary>
public class AnalyticsMessageConsumer : IConsumer<AnalyticsDataMessage>, IConsumer<ReportRequestMessage>
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly AnalyticsService _analyticsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsMessageConsumer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="analyticsService">The analytics service.</param>
    public AnalyticsMessageConsumer(
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        AnalyticsService analyticsService)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Consumes the analytics data message.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<AnalyticsDataMessage> context)
    {
        var message = context.Message;
        using var span = _telemetryProvider.CreateSpan("AnalyticsMessageConsumer.ConsumeAnalyticsData");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("analytics.category", message.Category);
        span.SetAttribute("analytics.name", message.Name);
        span.SetAttribute("analytics.value", message.Value);

        try
        {
            _logger.Info($"Received analytics data message: {message.Category}.{message.Name} ({message.Value})");

            await _analyticsService.RecordAnalyticsDataAsync(
                message.Category,
                message.Name,
                message.Value,
                message.Attributes);

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process analytics data message: {message.Category}.{message.Name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Consumes the report request message.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<ReportRequestMessage> context)
    {
        var message = context.Message;
        using var span = _telemetryProvider.CreateSpan("AnalyticsMessageConsumer.ConsumeReportRequest");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("report.type", message.ReportType);

        try
        {
            _logger.Info($"Received report request message: {message.ReportType}");

            var report = await _analyticsService.GenerateReportAsync(
                message.ReportType,
                message.Parameters);

            // Send the report back to the requester
            await context.RespondAsync(new ReportResponseMessage
            {
                CorrelationId = message.MessageId,
                ReportId = report.Id,
                ReportType = report.ReportType,
                Timestamp = DateTime.UtcNow,
                Data = report.Data
            });

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process report request message: {message.ReportType}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

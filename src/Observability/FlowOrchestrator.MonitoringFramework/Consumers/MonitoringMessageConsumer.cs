using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.MonitoringFramework.Messages;
using FlowOrchestrator.MonitoringFramework.Services;
using MassTransit;

namespace FlowOrchestrator.MonitoringFramework.Consumers;

/// <summary>
/// Consumer for monitoring messages.
/// </summary>
public class MonitoringMessageConsumer : IConsumer<HealthCheckMessage>, IConsumer<ResourceUtilizationMessage>
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly MonitoringService _monitoringService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitoringMessageConsumer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="monitoringService">The monitoring service.</param>
    public MonitoringMessageConsumer(
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        MonitoringService monitoringService)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _monitoringService = monitoringService;
    }

    /// <summary>
    /// Consumes the health check message.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<HealthCheckMessage> context)
    {
        var message = context.Message;
        using var span = _telemetryProvider.CreateSpan("MonitoringMessageConsumer.ConsumeHealthCheck");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("component.id", message.ComponentId);
        span.SetAttribute("health.check.name", message.CheckName);
        span.SetAttribute("health.check.status", message.Status.ToString());

        try
        {
            _logger.Info($"Received health check message: {message.ComponentId}.{message.CheckName} ({message.Status})");

            await _monitoringService.RecordHealthCheckAsync(
                message.ComponentId,
                message.CheckName,
                message.Status,
                message.Message,
                message.Details);

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process health check message: {message.ComponentId}.{message.CheckName}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Consumes the resource utilization message.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Consume(ConsumeContext<ResourceUtilizationMessage> context)
    {
        var message = context.Message;
        using var span = _telemetryProvider.CreateSpan("MonitoringMessageConsumer.ConsumeResourceUtilization");
        span.SetAttribute("message.id", message.MessageId);
        span.SetAttribute("component.id", message.ComponentId);
        span.SetAttribute("resource.type", message.ResourceType);
        span.SetAttribute("resource.value", message.Value);

        try
        {
            _logger.Info($"Received resource utilization message: {message.ComponentId}.{message.ResourceType} ({message.Value})");

            await _monitoringService.RecordResourceUtilizationAsync(
                message.ComponentId,
                message.ResourceType,
                message.Value,
                message.Attributes);

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process resource utilization message: {message.ComponentId}.{message.ResourceType}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}

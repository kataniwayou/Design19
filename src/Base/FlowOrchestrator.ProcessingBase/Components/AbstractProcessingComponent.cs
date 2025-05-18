using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;

namespace FlowOrchestrator.ProcessingBase.Components;

/// <summary>
/// Abstract base class for all processing components in the FlowOrchestrator system.
/// </summary>
public abstract class AbstractProcessingComponent
{
    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the configuration provider.
    /// </summary>
    protected IConfigurationProvider ConfigurationProvider { get; }

    /// <summary>
    /// Gets the message bus.
    /// </summary>
    protected IMessageBus MessageBus { get; }

    /// <summary>
    /// Gets the telemetry provider.
    /// </summary>
    protected ITelemetryProvider TelemetryProvider { get; }

    /// <summary>
    /// Gets the component identifier.
    /// </summary>
    public abstract string ComponentId { get; }

    /// <summary>
    /// Gets the component name.
    /// </summary>
    public abstract string ComponentName { get; }

    /// <summary>
    /// Gets the component type.
    /// </summary>
    public abstract string ComponentType { get; }

    /// <summary>
    /// Gets the component version.
    /// </summary>
    public abstract string Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractProcessingComponent"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractProcessingComponent(
        ILogger logger,
        IConfigurationProvider configurationProvider,
        IMessageBus messageBus,
        ITelemetryProvider telemetryProvider)
    {
        Logger = logger;
        ConfigurationProvider = configurationProvider;
        MessageBus = messageBus;
        TelemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Initializes the component.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InitializeAsync()
    {
        Logger.Info($"Initializing {ComponentType} component: {ComponentName} (ID: {ComponentId}, Version: {Version})");

        await RegisterComponentAsync();
    }

    /// <summary>
    /// Registers the component with the entity manager.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task RegisterComponentAsync()
    {
        using var span = TelemetryProvider.CreateSpan($"{ComponentType}.Register");
        span.SetAttribute("component.id", ComponentId);
        span.SetAttribute("component.name", ComponentName);
        span.SetAttribute("component.type", ComponentType);
        span.SetAttribute("component.version", Version);

        try
        {
            Logger.Info($"Registering {ComponentType} component: {ComponentName} (ID: {ComponentId}, Version: {Version})");

            var registrationMessage = CreateRegistrationMessage();
            await MessageBus.PublishAsync(registrationMessage);

            Logger.Info($"Registration message published for {ComponentType} component: {ComponentName}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to register {ComponentType} component: {ComponentName}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a registration message for the component.
    /// </summary>
    /// <returns>The registration message.</returns>
    protected abstract IMessage CreateRegistrationMessage();

    /// <summary>
    /// Sends a status message.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task SendStatusMessageAsync(string status, string message, string correlationId)
    {
        using var span = TelemetryProvider.CreateSpan($"{ComponentType}.SendStatus");
        span.SetAttribute("component.id", ComponentId);
        span.SetAttribute("component.name", ComponentName);
        span.SetAttribute("component.type", ComponentType);
        span.SetAttribute("status", status);
        span.SetAttribute("correlation.id", correlationId);

        try
        {
            Logger.Info($"Sending status message for {ComponentType} component: {ComponentName} - Status: {status}, Message: {message}");

            var statusMessage = CreateStatusMessage(status, message, correlationId);
            await MessageBus.PublishAsync(statusMessage);

            Logger.Info($"Status message published for {ComponentType} component: {ComponentName}");
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to send status message for {ComponentType} component: {ComponentName}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a status message.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>The status message.</returns>
    protected abstract IMessage CreateStatusMessage(string status, string message, string correlationId);
}

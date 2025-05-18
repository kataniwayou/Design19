using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.IntegrationBase.Messaging;

namespace FlowOrchestrator.IntegrationBase.Components;

/// <summary>
/// Abstract base class for all exporters in the FlowOrchestrator system.
/// </summary>
public abstract class AbstractExporterBase : AbstractIntegrationComponent
{
    /// <summary>
    /// Gets the component type.
    /// </summary>
    public override string ComponentType => "Exporter";

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractExporterBase"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractExporterBase(
        ILogger logger,
        IConfigurationProvider configurationProvider,
        IMessageBus messageBus,
        ITelemetryProvider telemetryProvider)
        : base(logger, configurationProvider, messageBus, telemetryProvider)
    {
    }

    /// <summary>
    /// Creates a registration message for the component.
    /// </summary>
    /// <returns>The registration message.</returns>
    protected override IMessage CreateRegistrationMessage()
    {
        return new ExporterRegistrationMessage(
            ComponentId,
            ComponentName,
            Version,
            GetProtocol(),
            GetDataFormat(),
            GetCapabilities(),
            GetSchemaDefinition(),
            Guid.NewGuid().ToString());
    }

    /// <summary>
    /// Creates a status message.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>The status message.</returns>
    protected override IMessage CreateStatusMessage(string status, string message, string correlationId)
    {
        return new ExporterStatusMessage(
            ComponentId,
            status,
            message,
            correlationId);
    }

    /// <summary>
    /// Exports data to a destination.
    /// </summary>
    /// <param name="parameters">The export parameters.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>The export result.</returns>
    public async Task<ExportResult> ExportAsync(ExportParameters parameters, string correlationId)
    {
        using var span = TelemetryProvider.CreateSpan("Exporter.Export");
        span.SetAttribute("component.id", ComponentId);
        span.SetAttribute("component.name", ComponentName);
        span.SetAttribute("correlation.id", correlationId);

        try
        {
            Logger.Info($"Starting export operation for {ComponentName} (ID: {ComponentId})");
            await SendStatusMessageAsync("Started", "Export operation started", correlationId);

            var result = await ExportInternalAsync(parameters);

            Logger.Info($"Export operation completed for {ComponentName} (ID: {ComponentId})");
            await SendStatusMessageAsync("Completed", "Export operation completed", correlationId);

            span.SetStatus(SpanStatus.Ok);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error($"Export operation failed for {ComponentName} (ID: {ComponentId})", ex);
            await SendStatusMessageAsync("Failed", $"Export operation failed: {ex.Message}", correlationId);

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);

            return new ExportResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Gets the protocol supported by the exporter.
    /// </summary>
    /// <returns>The protocol.</returns>
    protected abstract string GetProtocol();

    /// <summary>
    /// Gets the data format supported by the exporter.
    /// </summary>
    /// <returns>The data format.</returns>
    protected abstract string GetDataFormat();

    /// <summary>
    /// Gets the capabilities of the exporter.
    /// </summary>
    /// <returns>The capabilities.</returns>
    protected abstract List<string> GetCapabilities();

    /// <summary>
    /// Gets the schema definition of the exporter.
    /// </summary>
    /// <returns>The schema definition.</returns>
    protected abstract SchemaDefinition GetSchemaDefinition();

    /// <summary>
    /// Exports data to a destination.
    /// </summary>
    /// <param name="parameters">The export parameters.</param>
    /// <returns>The export result.</returns>
    protected abstract Task<ExportResult> ExportInternalAsync(ExportParameters parameters);
}

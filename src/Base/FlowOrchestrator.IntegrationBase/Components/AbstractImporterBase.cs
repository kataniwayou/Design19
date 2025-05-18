using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.IntegrationBase.Messaging;

namespace FlowOrchestrator.IntegrationBase.Components;

/// <summary>
/// Abstract base class for all importers in the FlowOrchestrator system.
/// </summary>
public abstract class AbstractImporterBase : AbstractIntegrationComponent
{
    /// <summary>
    /// Gets the component type.
    /// </summary>
    public override string ComponentType => "Importer";

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractImporterBase"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractImporterBase(
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
        return new ImporterRegistrationMessage(
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
        return new ImporterStatusMessage(
            ComponentId,
            status,
            message,
            correlationId);
    }

    /// <summary>
    /// Imports data from a source.
    /// </summary>
    /// <param name="parameters">The import parameters.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>The import result.</returns>
    public async Task<ImportResult> ImportAsync(ImportParameters parameters, string correlationId)
    {
        using var span = TelemetryProvider.CreateSpan("Importer.Import");
        span.SetAttribute("component.id", ComponentId);
        span.SetAttribute("component.name", ComponentName);
        span.SetAttribute("correlation.id", correlationId);

        try
        {
            Logger.Info($"Starting import operation for {ComponentName} (ID: {ComponentId})");
            await SendStatusMessageAsync("Started", "Import operation started", correlationId);

            var result = await ImportInternalAsync(parameters);

            Logger.Info($"Import operation completed for {ComponentName} (ID: {ComponentId})");
            await SendStatusMessageAsync("Completed", "Import operation completed", correlationId);

            span.SetStatus(SpanStatus.Ok);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error($"Import operation failed for {ComponentName} (ID: {ComponentId})", ex);
            await SendStatusMessageAsync("Failed", $"Import operation failed: {ex.Message}", correlationId);

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);

            return new ImportResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Data = null
            };
        }
    }

    /// <summary>
    /// Gets the protocol supported by the importer.
    /// </summary>
    /// <returns>The protocol.</returns>
    protected abstract string GetProtocol();

    /// <summary>
    /// Gets the data format supported by the importer.
    /// </summary>
    /// <returns>The data format.</returns>
    protected abstract string GetDataFormat();

    /// <summary>
    /// Gets the capabilities of the importer.
    /// </summary>
    /// <returns>The capabilities.</returns>
    protected abstract List<string> GetCapabilities();

    /// <summary>
    /// Gets the schema definition of the importer.
    /// </summary>
    /// <returns>The schema definition.</returns>
    protected abstract SchemaDefinition GetSchemaDefinition();

    /// <summary>
    /// Imports data from a source.
    /// </summary>
    /// <param name="parameters">The import parameters.</param>
    /// <returns>The import result.</returns>
    protected abstract Task<ImportResult> ImportInternalAsync(ImportParameters parameters);
}

using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.Common.Configuration;
using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ProcessingBase.Messaging;
using FlowOrchestrator.ProcessingBase.Schema;

namespace FlowOrchestrator.ProcessingBase.Components;

/// <summary>
/// Abstract base class for all processors in the FlowOrchestrator system.
/// </summary>
public abstract class AbstractProcessorBase : AbstractProcessingComponent
{
    /// <summary>
    /// Gets the component type.
    /// </summary>
    public override string ComponentType => "Processor";

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractProcessorBase"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    protected AbstractProcessorBase(
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
        return new ProcessorRegistrationMessage(
            ComponentId,
            ComponentName,
            Version,
            GetProcessorType(),
            GetCapabilities(),
            GetInputSchema(),
            GetOutputSchema(),
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
        return new ProcessorStatusMessage(
            ComponentId,
            status,
            message,
            correlationId);
    }

    /// <summary>
    /// Processes data.
    /// </summary>
    /// <param name="parameters">The process parameters.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <returns>The process result.</returns>
    public async Task<ProcessingResult> ProcessAsync(ProcessParameters parameters, string correlationId)
    {
        using var span = TelemetryProvider.CreateSpan("Processor.Process");
        span.SetAttribute("component.id", ComponentId);
        span.SetAttribute("component.name", ComponentName);
        span.SetAttribute("correlation.id", correlationId);

        try
        {
            Logger.Info($"Starting processing operation for {ComponentName} (ID: {ComponentId})");
            await SendStatusMessageAsync("Started", "Processing operation started", correlationId);

            var executionContext = new FlowOrchestrator.ProcessingBase.Messaging.ExecutionContext
            {
                CorrelationId = correlationId,
                FlowId = parameters.FlowId,
                StepId = parameters.StepId,
                BranchId = parameters.BranchId
            };

            var result = await ProcessInternalAsync(parameters, executionContext);

            Logger.Info($"Processing operation completed for {ComponentName} (ID: {ComponentId})");
            await SendStatusMessageAsync("Completed", "Processing operation completed", correlationId);

            span.SetStatus(SpanStatus.Ok);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error($"Processing operation failed for {ComponentName} (ID: {ComponentId})", ex);
            await SendStatusMessageAsync("Failed", $"Processing operation failed: {ex.Message}", correlationId);

            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);

            return new ProcessingResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                Data = null
            };
        }
    }

    /// <summary>
    /// Gets the processor type.
    /// </summary>
    /// <returns>The processor type.</returns>
    protected abstract string GetProcessorType();

    /// <summary>
    /// Gets the capabilities of the processor.
    /// </summary>
    /// <returns>The capabilities.</returns>
    protected abstract List<string> GetCapabilities();

    /// <summary>
    /// Gets the input schema of the processor.
    /// </summary>
    /// <returns>The input schema.</returns>
    protected abstract SchemaDefinition GetInputSchema();

    /// <summary>
    /// Gets the output schema of the processor.
    /// </summary>
    /// <returns>The output schema.</returns>
    protected abstract SchemaDefinition GetOutputSchema();

    /// <summary>
    /// Processes data.
    /// </summary>
    /// <param name="parameters">The process parameters.</param>
    /// <param name="context">The execution context.</param>
    /// <returns>The process result.</returns>
    protected abstract Task<ProcessingResult> ProcessInternalAsync(ProcessParameters parameters, FlowOrchestrator.ProcessingBase.Messaging.ExecutionContext context);
}

using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.ProcessingBase.Schema;

namespace FlowOrchestrator.ProcessingBase.Messaging;

/// <summary>
/// Represents a registration message for a processor.
/// </summary>
public class ProcessorRegistrationMessage : EventBase
{
    /// <summary>
    /// Gets the processor identifier.
    /// </summary>
    public string ProcessorId { get; }

    /// <summary>
    /// Gets the processor name.
    /// </summary>
    public string ProcessorName { get; }

    /// <summary>
    /// Gets the processor version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the processor type.
    /// </summary>
    public string ProcessorType { get; }

    /// <summary>
    /// Gets the capabilities of the processor.
    /// </summary>
    public List<string> Capabilities { get; }

    /// <summary>
    /// Gets the input schema of the processor.
    /// </summary>
    public SchemaDefinition InputSchema { get; }

    /// <summary>
    /// Gets the output schema of the processor.
    /// </summary>
    public SchemaDefinition OutputSchema { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorRegistrationMessage"/> class.
    /// </summary>
    /// <param name="processorId">The processor identifier.</param>
    /// <param name="processorName">The processor name.</param>
    /// <param name="version">The processor version.</param>
    /// <param name="processorType">The processor type.</param>
    /// <param name="capabilities">The capabilities of the processor.</param>
    /// <param name="inputSchema">The input schema of the processor.</param>
    /// <param name="outputSchema">The output schema of the processor.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ProcessorRegistrationMessage(
        string processorId,
        string processorName,
        string version,
        string processorType,
        List<string> capabilities,
        SchemaDefinition inputSchema,
        SchemaDefinition outputSchema,
        string correlationId)
        : base(correlationId)
    {
        ProcessorId = processorId;
        ProcessorName = processorName;
        Version = version;
        ProcessorType = processorType;
        Capabilities = capabilities;
        InputSchema = inputSchema;
        OutputSchema = outputSchema;
    }
}

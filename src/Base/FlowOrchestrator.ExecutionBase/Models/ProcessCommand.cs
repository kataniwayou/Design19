using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.ProcessingBase.Messaging;

namespace FlowOrchestrator.ExecutionBase.Models;

/// <summary>
/// Represents a process command.
/// </summary>
public class ProcessCommand : CommandBase
{
    /// <summary>
    /// Gets the processor identifier.
    /// </summary>
    public string ProcessorId { get; }

    /// <summary>
    /// Gets the process parameters.
    /// </summary>
    public ProcessParameters Parameters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessCommand"/> class.
    /// </summary>
    /// <param name="processorId">The processor identifier.</param>
    /// <param name="parameters">The process parameters.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ProcessCommand(
        string processorId,
        ProcessParameters parameters,
        string correlationId)
        : base(correlationId)
    {
        ProcessorId = processorId;
        Parameters = parameters;
    }
}

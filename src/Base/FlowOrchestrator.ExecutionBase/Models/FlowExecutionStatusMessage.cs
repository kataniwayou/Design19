using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.ExecutionBase.Models;

/// <summary>
/// Represents a flow execution status message.
/// </summary>
public class FlowExecutionStatusMessage : EventBase
{
    /// <summary>
    /// Gets the execution identifier.
    /// </summary>
    public string ExecutionId { get; }

    /// <summary>
    /// Gets the flow identifier.
    /// </summary>
    public string FlowId { get; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowExecutionStatusMessage"/> class.
    /// </summary>
    /// <param name="executionId">The execution identifier.</param>
    /// <param name="flowId">The flow identifier.</param>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public FlowExecutionStatusMessage(
        string executionId,
        string flowId,
        string status,
        string message,
        string correlationId)
        : base(correlationId)
    {
        ExecutionId = executionId;
        FlowId = flowId;
        Status = status;
        Message = message;
    }
}

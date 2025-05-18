namespace FlowOrchestrator.ProcessingBase.Messaging;

/// <summary>
/// Represents the execution context for a process operation.
/// </summary>
public class ExecutionContext
{
    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the flow identifier.
    /// </summary>
    public string FlowId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the step identifier.
    /// </summary>
    public string StepId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch identifier.
    /// </summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the execution context variables.
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
}

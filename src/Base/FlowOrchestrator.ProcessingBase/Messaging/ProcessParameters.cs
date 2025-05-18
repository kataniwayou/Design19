namespace FlowOrchestrator.ProcessingBase.Messaging;

/// <summary>
/// Represents parameters for a process operation.
/// </summary>
public class ProcessParameters
{
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
    /// Gets or sets the data to process.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the data format.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the process configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
}

using FlowOrchestrator.IntegrationBase.Messaging;
using FlowOrchestrator.ProcessingBase.Messaging;

namespace FlowOrchestrator.ExecutionBase.Models;

/// <summary>
/// Represents the execution context for a flow execution.
/// </summary>
public class ExecutionContext
{
    /// <summary>
    /// Gets or sets the execution identifier.
    /// </summary>
    public string ExecutionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the flow identifier.
    /// </summary>
    public string FlowId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scheduled flow identifier.
    /// </summary>
    public string? ScheduledFlowId { get; set; }

    /// <summary>
    /// Gets or sets the source assignment identifier.
    /// </summary>
    public string SourceAssignmentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination assignment identifiers.
    /// </summary>
    public List<string> DestinationAssignmentIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the execution start time.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the execution end time.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the execution status.
    /// </summary>
    public ExecutionStatus Status { get; set; } = ExecutionStatus.Created;

    /// <summary>
    /// Gets or sets the execution error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the execution variables.
    /// </summary>
    public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the execution steps.
    /// </summary>
    public List<ExecutionStep> Steps { get; set; } = new List<ExecutionStep>();

    /// <summary>
    /// Gets or sets the execution branches.
    /// </summary>
    public List<ExecutionBranch> Branches { get; set; } = new List<ExecutionBranch>();

    /// <summary>
    /// Gets or sets the import result.
    /// </summary>
    public ImportResult? ImportResult { get; set; }

    /// <summary>
    /// Gets or sets the export results.
    /// </summary>
    public Dictionary<string, ExportResult> ExportResults { get; set; } = new Dictionary<string, ExportResult>();

    /// <summary>
    /// Gets or sets the processing results.
    /// </summary>
    public Dictionary<string, ProcessingResult> ProcessingResults { get; set; } = new Dictionary<string, ProcessingResult>();

    /// <summary>
    /// Gets the execution duration.
    /// </summary>
    public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : DateTime.UtcNow - StartTime;
}

/// <summary>
/// Represents the status of an execution.
/// </summary>
public enum ExecutionStatus
{
    /// <summary>
    /// The execution has been created.
    /// </summary>
    Created,

    /// <summary>
    /// The execution is initializing.
    /// </summary>
    Initializing,

    /// <summary>
    /// The execution is importing data.
    /// </summary>
    Importing,

    /// <summary>
    /// The execution is processing data.
    /// </summary>
    Processing,

    /// <summary>
    /// The execution is exporting data.
    /// </summary>
    Exporting,

    /// <summary>
    /// The execution has completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The execution has failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The execution has been cancelled.
    /// </summary>
    Cancelled
}

/// <summary>
/// Represents an execution step.
/// </summary>
public class ExecutionStep
{
    /// <summary>
    /// Gets or sets the step identifier.
    /// </summary>
    public string StepId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the step name.
    /// </summary>
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the step type.
    /// </summary>
    public string StepType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the step start time.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the step end time.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the step status.
    /// </summary>
    public ExecutionStepStatus Status { get; set; } = ExecutionStepStatus.Created;

    /// <summary>
    /// Gets or sets the step error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the step result.
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// Gets the step duration.
    /// </summary>
    public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : DateTime.UtcNow - StartTime;
}

/// <summary>
/// Represents the status of an execution step.
/// </summary>
public enum ExecutionStepStatus
{
    /// <summary>
    /// The step has been created.
    /// </summary>
    Created,

    /// <summary>
    /// The step is running.
    /// </summary>
    Running,

    /// <summary>
    /// The step has completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The step has failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The step has been skipped.
    /// </summary>
    Skipped
}

/// <summary>
/// Represents an execution branch.
/// </summary>
public class ExecutionBranch
{
    /// <summary>
    /// Gets or sets the branch identifier.
    /// </summary>
    public string BranchId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the processing chain identifier.
    /// </summary>
    public string ProcessingChainId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination assignment identifier.
    /// </summary>
    public string DestinationAssignmentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch start time.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the branch end time.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the branch status.
    /// </summary>
    public ExecutionBranchStatus Status { get; set; } = ExecutionBranchStatus.Created;

    /// <summary>
    /// Gets or sets the branch error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the branch steps.
    /// </summary>
    public List<ExecutionStep> Steps { get; set; } = new List<ExecutionStep>();

    /// <summary>
    /// Gets the branch duration.
    /// </summary>
    public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : DateTime.UtcNow - StartTime;
}

/// <summary>
/// Represents the status of an execution branch.
/// </summary>
public enum ExecutionBranchStatus
{
    /// <summary>
    /// The branch has been created.
    /// </summary>
    Created,

    /// <summary>
    /// The branch is running.
    /// </summary>
    Running,

    /// <summary>
    /// The branch has completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The branch has failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The branch has been skipped.
    /// </summary>
    Skipped
}

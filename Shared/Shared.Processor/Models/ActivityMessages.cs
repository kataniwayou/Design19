using Shared.Entities.Base;
using Shared.Entities;

namespace Shared.Processor.Models;

/// <summary>
/// Message for executing an activity in the processor
/// </summary>
public class ProcessorActivityMessage
{
    /// <summary>
    /// ID of the processor that should handle this activity
    /// </summary>
    public Guid ProcessorId { get; set; }

    /// <summary>
    /// ID of the orchestrated flow entity
    /// </summary>
    public Guid OrchestratedFlowEntityId { get; set; }

    /// <summary>
    /// ID of the step being executed
    /// </summary>
    public Guid StepId { get; set; }

    /// <summary>
    /// Unique execution ID for this activity instance
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Collection of assignment models to process
    /// </summary>
    public List<AssignmentModel> Entities { get; set; } = new();

    /// <summary>
    /// Correlation ID for tracking (defaults to Guid.Empty)
    /// </summary>
    public Guid CorrelationId { get; set; } = Guid.Empty;

    /// <summary>
    /// Timestamp when the message was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Response message after processing an activity
/// </summary>
public class ProcessorActivityResponse
{
    /// <summary>
    /// ID of the processor that handled this activity
    /// </summary>
    public Guid ProcessorId { get; set; }

    /// <summary>
    /// ID of the orchestrated flow entity
    /// </summary>
    public Guid OrchestratedFlowEntityId { get; set; }

    /// <summary>
    /// ID of the step that was executed
    /// </summary>
    public Guid StepId { get; set; }

    /// <summary>
    /// Execution ID for this activity instance
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// Status of the activity execution
    /// </summary>
    public ActivityExecutionStatus Status { get; set; }

    /// <summary>
    /// Correlation ID for tracking (defaults to Guid.Empty)
    /// </summary>
    public Guid CorrelationId { get; set; } = Guid.Empty;

    /// <summary>
    /// Optional error message if execution failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Timestamp when the activity was completed
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Duration of the activity execution
    /// </summary>
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Health check message for processor
/// </summary>
public class ProcessorHealthCheckMessage
{
    /// <summary>
    /// ID of the processor to check
    /// </summary>
    public Guid ProcessorId { get; set; }

    /// <summary>
    /// Request ID for tracking
    /// </summary>
    public Guid RequestId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the request was made
    /// </summary>
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Statistics request message for processor
/// </summary>
public class ProcessorStatisticsMessage
{
    /// <summary>
    /// ID of the processor to get statistics for
    /// </summary>
    public Guid ProcessorId { get; set; }

    /// <summary>
    /// Request ID for tracking
    /// </summary>
    public Guid RequestId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Start date for statistics period (null for all time)
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// End date for statistics period (null for current time)
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Timestamp when the request was made
    /// </summary>
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Status of activity execution
/// </summary>
public enum ActivityExecutionStatus
{
    /// <summary>
    /// Activity is currently being processed
    /// </summary>
    Processing,

    /// <summary>
    /// Activity completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// Activity failed with an error
    /// </summary>
    Failed,

    /// <summary>
    /// Activity was cancelled
    /// </summary>
    Cancelled
}

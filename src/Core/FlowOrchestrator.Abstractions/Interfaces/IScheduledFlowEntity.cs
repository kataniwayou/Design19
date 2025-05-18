namespace FlowOrchestrator.Abstractions.Interfaces;

/// <summary>
/// Defines the contract for a scheduled flow entity in the FlowOrchestrator system.
/// A scheduled flow entity represents a flow that is scheduled to run at specific times.
/// </summary>
public interface IScheduledFlowEntity : IEntity
{
    /// <summary>
    /// Gets the identifier of the flow.
    /// </summary>
    string FlowId { get; }

    /// <summary>
    /// Gets the identifier of the source assignment.
    /// </summary>
    string SourceAssignmentId { get; }

    /// <summary>
    /// Gets the identifiers of the destination assignments.
    /// </summary>
    IReadOnlyList<string> DestinationAssignmentIds { get; }

    /// <summary>
    /// Gets the schedule expression.
    /// </summary>
    string ScheduleExpression { get; }

    /// <summary>
    /// Gets the schedule type.
    /// </summary>
    ScheduleType ScheduleType { get; }

    /// <summary>
    /// Gets a value indicating whether the scheduled flow is enabled.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Gets the schedule configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> Configuration { get; }
}

/// <summary>
/// Defines the type of schedule.
/// </summary>
public enum ScheduleType
{
    /// <summary>
    /// A schedule based on a cron expression.
    /// </summary>
    Cron,

    /// <summary>
    /// A schedule based on a fixed interval.
    /// </summary>
    Interval,

    /// <summary>
    /// A schedule based on a specific date and time.
    /// </summary>
    OneTime,

    /// <summary>
    /// A schedule based on an external trigger.
    /// </summary>
    Trigger
}

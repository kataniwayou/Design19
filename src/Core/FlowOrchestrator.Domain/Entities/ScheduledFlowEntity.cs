using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a scheduled flow entity in the FlowOrchestrator system.
/// A scheduled flow entity represents a flow that is scheduled to run at specific times.
/// </summary>
public class ScheduledFlowEntity : AbstractEntity, IScheduledFlowEntity
{
    /// <summary>
    /// Gets or sets the identifier of the flow.
    /// </summary>
    public string FlowId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the source assignment.
    /// </summary>
    public string SourceAssignmentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifiers of the destination assignments.
    /// </summary>
    public List<string> DestinationAssignmentIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the schedule expression.
    /// </summary>
    public string ScheduleExpression { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schedule.
    /// </summary>
    public string Schedule { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schedule type.
    /// </summary>
    public ScheduleType ScheduleType { get; set; } = ScheduleType.Cron;

    /// <summary>
    /// Gets or sets a value indicating whether the scheduled flow is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the schedule configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the identifiers of the destination assignments.
    /// </summary>
    IReadOnlyList<string> IScheduledFlowEntity.DestinationAssignmentIds => DestinationAssignmentIds.AsReadOnly();

    /// <summary>
    /// Gets the schedule configuration.
    /// </summary>
    IReadOnlyDictionary<string, object> IScheduledFlowEntity.Configuration => Configuration;
}

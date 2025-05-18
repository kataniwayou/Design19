using FlowOrchestrator.Abstractions.Interfaces;
using FlowOrchestrator.Domain.Entities;
using System.Threading.Tasks;

namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Validator for scheduled flow entities.
/// </summary>
public class ScheduledFlowValidator : IValidator<ScheduledFlowEntity>
{
    private readonly IRepository<FlowEntity> _flowRepository;
    private readonly IRepository<SourceAssignmentEntity> _sourceAssignmentRepository;
    private readonly IRepository<DestinationAssignmentEntity> _destinationAssignmentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledFlowValidator"/> class.
    /// </summary>
    /// <param name="flowRepository">The flow repository.</param>
    /// <param name="sourceAssignmentRepository">The source assignment repository.</param>
    /// <param name="destinationAssignmentRepository">The destination assignment repository.</param>
    public ScheduledFlowValidator(
        IRepository<FlowEntity> flowRepository,
        IRepository<SourceAssignmentEntity> sourceAssignmentRepository,
        IRepository<DestinationAssignmentEntity> destinationAssignmentRepository)
    {
        _flowRepository = flowRepository;
        _sourceAssignmentRepository = sourceAssignmentRepository;
        _destinationAssignmentRepository = destinationAssignmentRepository;
    }

    /// <summary>
    /// Validates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public ValidationResult Validate(ScheduledFlowEntity entity)
    {
        return ValidateAsync(entity).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Validates the specified entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>The validation result.</returns>
    public async Task<ValidationResult> ValidateAsync(ScheduledFlowEntity entity)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(entity.FlowId))
        {
            result.AddError("FlowId", "Flow ID is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.SourceAssignmentId))
        {
            result.AddError("SourceAssignmentId", "Source assignment ID is required.");
        }

        if (entity.DestinationAssignmentIds == null || entity.DestinationAssignmentIds.Count == 0)
        {
            result.AddError("DestinationAssignmentIds", "At least one destination assignment ID is required.");
        }

        if (string.IsNullOrWhiteSpace(entity.Schedule))
        {
            result.AddError("Schedule", "Schedule is required.");
        }
        else if (!IsValidCronExpression(entity.Schedule))
        {
            result.AddError("Schedule", "Invalid cron expression.");
        }

        // If basic validation fails, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate that flow, source assignment, and destination assignments exist
        var flow = await _flowRepository.GetByIdAsync(entity.FlowId);
        var sourceAssignment = await _sourceAssignmentRepository.GetByIdAsync(entity.SourceAssignmentId);

        if (flow == null)
        {
            result.AddError("FlowId", $"Flow with ID {entity.FlowId} not found.");
        }

        if (sourceAssignment == null)
        {
            result.AddError("SourceAssignmentId", $"Source assignment with ID {entity.SourceAssignmentId} not found.");
        }

        foreach (var destinationAssignmentId in entity.DestinationAssignmentIds)
        {
            var destinationAssignment = await _destinationAssignmentRepository.GetByIdAsync(destinationAssignmentId);
            if (destinationAssignment == null)
            {
                result.AddError("DestinationAssignmentIds", $"Destination assignment with ID {destinationAssignmentId} not found.");
            }
        }

        // If any entities don't exist, return early
        if (!result.IsValid)
        {
            return result;
        }

        // Validate that the flow is active
        if (flow.Status != "Active")
        {
            result.AddError("FlowId", $"Flow {flow.Name} is not active.");
        }

        return result;
    }

    private bool IsValidCronExpression(string cronExpression)
    {
        // Simple validation for cron expressions
        // A more comprehensive validation would use a cron expression parser library
        var parts = cronExpression.Split(' ');
        return parts.Length == 5 || parts.Length == 6;
    }
}

using Shared.Processor.MassTransit.Commands;
using Shared.Extensions;
using Shared.Processor.MassTransit.Events;
using Shared.Processor.Models;
using Shared.Processor.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Shared.Models;
namespace Shared.Processor.MassTransit.Consumers;

/// <summary>
/// Consumer for ExecuteActivityCommand messages
/// </summary>
public class ExecuteActivityCommandConsumer : IConsumer<ExecuteActivityCommand>
{
    private readonly IProcessorService _processorService;
    private readonly ILogger<ExecuteActivityCommandConsumer> _logger;
    private static readonly ActivitySource ActivitySource = new(ActivitySources.Services);

    public ExecuteActivityCommandConsumer(
        IProcessorService processorService,
        ILogger<ExecuteActivityCommandConsumer> logger)
    {
        _processorService = processorService;
        _logger = logger;

        // Add debug logging to verify consumer is being created
        _logger.LogInformation("ExecuteActivityCommandConsumer created and registered successfully");
        Console.WriteLine("✅ ExecuteActivityCommandConsumer instantiated");
    }

    public async Task Consume(ConsumeContext<ExecuteActivityCommand> context)
    {
        using var activity = ActivitySource.StartActivity("ExecuteActivityCommand");
        var command = context.Message;
        var stopwatch = Stopwatch.StartNew();

        activity?.SetMessageTags(nameof(ExecuteActivityCommand), nameof(ExecuteActivityCommandConsumer), command.CorrelationId)
                ?.SetActivityExecutionTags(
                    command.OrchestratedFlowEntityId,
                    command.StepId,
                    command.ExecutionId,
                    command.CorrelationId)
                ?.SetEntityTags(command.Entities.Count);

        _logger.LogInformation(
            "Processing ExecuteActivityCommand. ProcessorId: {ProcessorId}, OrchestratedFlowEntityId: {OrchestratedFlowEntityId}, StepId: {StepId}, ExecutionId: {ExecutionId}",
            command.ProcessorId, command.OrchestratedFlowEntityId, command.StepId, command.ExecutionId);

        try
        {
            // Get current processor ID for debugging
            var currentProcessorId = await _processorService.GetProcessorIdAsync();

            _logger.LogInformation(
                "Received ExecuteActivityCommand. TargetProcessorId: {TargetProcessorId}, CurrentProcessorId: {CurrentProcessorId}, OrchestratedFlowEntityId: {OrchestratedFlowEntityId}, StepId: {StepId}, ExecutionId: {ExecutionId}",
                command.ProcessorId, currentProcessorId, command.OrchestratedFlowEntityId, command.StepId, command.ExecutionId);

            // Special handling for uninitialized processor
            if (currentProcessorId == Guid.Empty)
            {
                _logger.LogWarning(
                    "Processor not yet initialized (ProcessorId is empty). Rejecting message and requeueing. TargetProcessorId: {TargetProcessorId}, OrchestratedFlowEntityId: {OrchestratedFlowEntityId}, StepId: {StepId}, ExecutionId: {ExecutionId}",
                    command.ProcessorId, command.OrchestratedFlowEntityId, command.StepId, command.ExecutionId);

                // Throw an exception to trigger MassTransit retry mechanism
                throw new InvalidOperationException($"Processor not yet initialized. ProcessorId is empty. Message will be retried.");
            }

            // Check if this message is for this processor instance
            if (!await _processorService.IsMessageForThisProcessorAsync(command.ProcessorId))
            {
                _logger.LogWarning(
                    "Message not for this processor instance. TargetProcessorId: {TargetProcessorId}, CurrentProcessorId: {CurrentProcessorId}, OrchestratedFlowEntityId: {OrchestratedFlowEntityId}, StepId: {StepId}, ExecutionId: {ExecutionId}",
                    command.ProcessorId, currentProcessorId, command.OrchestratedFlowEntityId, command.StepId, command.ExecutionId);
                return;
            }

            // Convert command to activity message
            var activityMessage = new ProcessorActivityMessage
            {
                ProcessorId = command.ProcessorId,
                OrchestratedFlowEntityId = command.OrchestratedFlowEntityId,
                StepId = command.StepId,
                ExecutionId = command.ExecutionId,
                Entities = command.Entities,
                CorrelationId = command.CorrelationId,
                CreatedAt = command.CreatedAt
            };

            // Process the activity
            var response = await _processorService.ProcessActivityAsync(activityMessage);

            stopwatch.Stop();

            // Set success telemetry
            activity?.SetTag(ActivityTags.ActivityStatus, response.Status.ToString())
                    ?.SetTag(ActivityTags.ActivityDuration, stopwatch.ElapsedMilliseconds);

            _logger.LogInformation(
                "Successfully executed activity. ProcessorId: {ProcessorId}, OrchestratedFlowEntityId: {OrchestratedFlowEntityId}, StepId: {StepId}, ExecutionId: {ExecutionId}, Status: {Status}, Duration: {Duration}ms",
                command.ProcessorId, command.OrchestratedFlowEntityId, command.StepId, command.ExecutionId, response.Status, stopwatch.ElapsedMilliseconds);

            // Publish response message
            await context.Publish(response);

            // Publish success event
            if (response.Status == ActivityExecutionStatus.Completed)
            {
                await context.Publish(new ActivityExecutedEvent
                {
                    ProcessorId = response.ProcessorId,
                    OrchestratedFlowEntityId = response.OrchestratedFlowEntityId,
                    StepId = response.StepId,
                    ExecutionId = response.ExecutionId,
                    CorrelationId = response.CorrelationId,
                    Duration = response.Duration,
                    Status = response.Status,
                    EntitiesProcessed = command.Entities.Count
                });
            }
            else
            {
                // Publish failure event
                await context.Publish(new ActivityFailedEvent
                {
                    ProcessorId = response.ProcessorId,
                    OrchestratedFlowEntityId = response.OrchestratedFlowEntityId,
                    StepId = response.StepId,
                    ExecutionId = response.ExecutionId,
                    CorrelationId = response.CorrelationId,
                    Duration = response.Duration,
                    ErrorMessage = response.ErrorMessage ?? "Unknown error",
                    EntitiesBeingProcessed = command.Entities.Count
                });
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            activity?.SetErrorTags(ex)
                    ?.SetTag(ActivityTags.ActivityStatus, ActivityExecutionStatus.Failed.ToString())
                    ?.SetTag(ActivityTags.ActivityDuration, stopwatch.ElapsedMilliseconds);

            _logger.LogError(ex,
                "Failed to execute activity. ProcessorId: {ProcessorId}, OrchestratedFlowEntityId: {OrchestratedFlowEntityId}, StepId: {StepId}, ExecutionId: {ExecutionId}, Duration: {Duration}ms",
                command.ProcessorId, command.OrchestratedFlowEntityId, command.StepId, command.ExecutionId, stopwatch.ElapsedMilliseconds);

            // Publish error response
            var errorResponse = new ProcessorActivityResponse
            {
                ProcessorId = command.ProcessorId,
                OrchestratedFlowEntityId = command.OrchestratedFlowEntityId,
                StepId = command.StepId,
                ExecutionId = command.ExecutionId,
                Status = ActivityExecutionStatus.Failed,
                CorrelationId = command.CorrelationId,
                ErrorMessage = ex.Message,
                Duration = stopwatch.Elapsed
            };

            await context.Publish(errorResponse);

            // Publish failure event
            await context.Publish(new ActivityFailedEvent
            {
                ProcessorId = command.ProcessorId,
                OrchestratedFlowEntityId = command.OrchestratedFlowEntityId,
                StepId = command.StepId,
                ExecutionId = command.ExecutionId,
                CorrelationId = command.CorrelationId,
                Duration = stopwatch.Elapsed,
                ErrorMessage = ex.Message,
                ExceptionType = ex.GetType().Name,
                StackTrace = ex.StackTrace,
                EntitiesBeingProcessed = command.Entities.Count
            });

            // Re-throw to trigger MassTransit error handling
            throw;
        }
    }
}

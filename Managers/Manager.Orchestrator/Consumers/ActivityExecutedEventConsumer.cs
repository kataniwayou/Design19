using Manager.Orchestrator.Models;
using Manager.Orchestrator.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Entities;
using Shared.Processor.MassTransit.Commands;
using Shared.Processor.MassTransit.Events;
using Shared.Services;
using System.Diagnostics;

namespace Manager.Orchestrator.Consumers;

/// <summary>
/// Consumer for ActivityExecutedEvent that handles workflow progression
/// Manages the transition from completed steps to their next steps
/// </summary>
public class ActivityExecutedEventConsumer : IConsumer<ActivityExecutedEvent>
{
    private readonly IOrchestrationCacheService _orchestrationCacheService;
    private readonly ICacheService _rawCacheService;
    private readonly ILogger<ActivityExecutedEventConsumer> _logger;
    private readonly IBus _bus;
    private static readonly ActivitySource ActivitySource = new("Manager.Orchestrator.Consumers");

    public ActivityExecutedEventConsumer(
        IOrchestrationCacheService orchestrationCacheService,
        ICacheService rawCacheService,
        ILogger<ActivityExecutedEventConsumer> logger,
        IBus bus)
    {
        _orchestrationCacheService = orchestrationCacheService;
        _rawCacheService = rawCacheService;
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<ActivityExecutedEvent> context)
    {
        var activityEvent = context.Message;
        var correlationId = Guid.NewGuid();

        using var activity = ActivitySource.StartActivity("ProcessActivityExecutedEvent");
        activity?.SetTag("orchestratedFlowId", activityEvent.OrchestratedFlowEntityId.ToString())
                ?.SetTag("stepId", activityEvent.StepId.ToString())
                ?.SetTag("processorId", activityEvent.ProcessorId.ToString())
                ?.SetTag("executionId", activityEvent.ExecutionId.ToString())
                ?.SetTag("correlationId", correlationId.ToString())
                ?.SetTag("eventCorrelationId", activityEvent.CorrelationId.ToString());

        var stopwatch = Stopwatch.StartNew();
        var publishedCommands = 0;

        _logger.LogInformation("Processing ActivityExecutedEvent. OrchestratedFlowId: {OrchestratedFlowId}, StepId: {StepId}, ProcessorId: {ProcessorId}, ExecutionId: {ExecutionId}, EventCorrelationId: {EventCorrelationId}",
            activityEvent.OrchestratedFlowEntityId, activityEvent.StepId, activityEvent.ProcessorId, activityEvent.ExecutionId, activityEvent.CorrelationId);

        try
        {
            // Step 1: Read OrchestrationCacheModel from cache
            activity?.SetTag("step", "1-ReadOrchestrationCache");
            var orchestrationData = await _orchestrationCacheService.GetOrchestrationDataAsync(activityEvent.OrchestratedFlowEntityId);
            if (orchestrationData == null)
            {
                throw new InvalidOperationException($"Orchestration data not found in cache for OrchestratedFlowId: {activityEvent.OrchestratedFlowEntityId}");
            }

            // Step 2: Get the nextSteps collection from StepEntities
            activity?.SetTag("step", "2-GetNextSteps");
            if (!orchestrationData.StepManager.StepEntities.TryGetValue(activityEvent.StepId, out var currentStepEntity))
            {
                throw new InvalidOperationException($"Step entity not found for StepId: {activityEvent.StepId}");
            }

            var nextSteps = currentStepEntity.NextStepIds;
            activity?.SetTag("nextStepCount", nextSteps.Count);

            _logger.LogDebug("Found {NextStepCount} next steps for StepId: {StepId}",
                nextSteps.Count, activityEvent.StepId);

            // Step 3: Check if nextSteps collection is empty (flow branch termination)
            if (nextSteps.Count == 0)
            {
                activity?.SetTag("step", "3-FlowTermination");
                await HandleFlowBranchTerminationAsync(activityEvent);
                
                stopwatch.Stop();
                activity?.SetTag("result", "FlowTerminated")
                        ?.SetTag("duration.ms", stopwatch.ElapsedMilliseconds)
                        ?.SetStatus(ActivityStatusCode.Ok);

                _logger.LogInformation("Flow branch termination detected and processed. OrchestratedFlowId: {OrchestratedFlowId}, StepId: {StepId}, Duration: {Duration}ms",
                    activityEvent.OrchestratedFlowEntityId, activityEvent.StepId, stopwatch.ElapsedMilliseconds);
                return;
            }

            // Step 4: Process each next step
            activity?.SetTag("step", "4-ProcessNextSteps");
            await ProcessNextStepsAsync(activityEvent, nextSteps, orchestrationData);
            publishedCommands = nextSteps.Count;

            stopwatch.Stop();
            activity?.SetTag("publishedCommands", publishedCommands)
                    ?.SetTag("duration.ms", stopwatch.ElapsedMilliseconds)
                    ?.SetTag("result", "Success")
                    ?.SetStatus(ActivityStatusCode.Ok);

            _logger.LogInformation("Successfully processed ActivityExecutedEvent. OrchestratedFlowId: {OrchestratedFlowId}, StepId: {StepId}, NextSteps: {NextStepCount}, PublishedCommands: {PublishedCommands}, Duration: {Duration}ms",
                activityEvent.OrchestratedFlowEntityId, activityEvent.StepId, nextSteps.Count, publishedCommands, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message)
                    ?.SetTag("duration.ms", stopwatch.ElapsedMilliseconds)
                    ?.SetTag("error.type", ex.GetType().Name)
                    ?.SetTag("result", "Error");

            _logger.LogError(ex, "Error processing ActivityExecutedEvent. OrchestratedFlowId: {OrchestratedFlowId}, StepId: {StepId}, ProcessorId: {ProcessorId}, Duration: {Duration}ms, ErrorType: {ErrorType}",
                activityEvent.OrchestratedFlowEntityId, activityEvent.StepId, activityEvent.ProcessorId, stopwatch.ElapsedMilliseconds, ex.GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Handles flow branch termination by deleting cache processor data
    /// </summary>
    /// <param name="activityEvent">The activity executed event</param>
    private async Task HandleFlowBranchTerminationAsync(ActivityExecutedEvent activityEvent)
    {
        using var activity = ActivitySource.StartActivity("HandleFlowBranchTermination");
        activity?.SetTag("orchestratedFlowId", activityEvent.OrchestratedFlowEntityId.ToString())
                ?.SetTag("stepId", activityEvent.StepId.ToString())
                ?.SetTag("processorId", activityEvent.ProcessorId.ToString());

        try
        {
            // Delete cache processor data using the standardized cache key method
            var mapName = activityEvent.ProcessorId.ToString();
            var key = _rawCacheService.GetCacheKey(activityEvent.OrchestratedFlowEntityId, activityEvent.StepId, activityEvent.ExecutionId, activityEvent.CorrelationId);

            await _rawCacheService.RemoveAsync(mapName, key);

            _logger.LogInformation("Deleted cache processor data for flow branch termination. MapName: {MapName}, Key: {Key}",
                mapName, key);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _logger.LogError(ex, "Failed to delete cache processor data for flow branch termination. ProcessorId: {ProcessorId}, OrchestratedFlowId: {OrchestratedFlowId}, StepId: {StepId}",
                activityEvent.ProcessorId, activityEvent.OrchestratedFlowEntityId, activityEvent.StepId);
            throw;
        }
    }

    /// <summary>
    /// Processes all next steps by copying cache data and publishing ExecuteActivityCommand
    /// </summary>
    /// <param name="activityEvent">The activity executed event</param>
    /// <param name="nextSteps">Collection of next step IDs</param>
    /// <param name="orchestrationData">Orchestration cache data</param>
    private async Task ProcessNextStepsAsync(ActivityExecutedEvent activityEvent, List<Guid> nextSteps, OrchestrationCacheModel orchestrationData)
    {
        using var activity = ActivitySource.StartActivity("ProcessNextSteps");
        activity?.SetTag("nextStepCount", nextSteps.Count);

        var tasks = new List<Task>();

        foreach (var nextStepId in nextSteps)
        {
            var task = ProcessSingleNextStepAsync(activityEvent, nextStepId, orchestrationData);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        activity?.SetStatus(ActivityStatusCode.Ok);
    }

    /// <summary>
    /// Processes a single next step by copying cache data and publishing ExecuteActivityCommand
    /// </summary>
    /// <param name="activityEvent">The activity executed event</param>
    /// <param name="nextStepId">The next step ID to process</param>
    /// <param name="orchestrationData">Orchestration cache data</param>
    private async Task ProcessSingleNextStepAsync(ActivityExecutedEvent activityEvent, Guid nextStepId, OrchestrationCacheModel orchestrationData)
    {
        using var activity = ActivitySource.StartActivity("ProcessSingleNextStep");
        activity?.SetTag("nextStepId", nextStepId.ToString());

        try
        {
            // Get next step entity
            if (!orchestrationData.StepManager.StepEntities.TryGetValue(nextStepId, out var nextStepEntity))
            {
                throw new InvalidOperationException($"Next step entity not found for StepId: {nextStepId}");
            }

            // Step 4.1: Copy cache processor data from source to destination
            await CopyCacheProcessorDataAsync(activityEvent, nextStepId, nextStepEntity.ProcessorId);

            // Step 4.2: Get assignments for next step
            var assignmentModels = new List<AssignmentModel>();
            if (orchestrationData.AssignmentManager.Assignments.TryGetValue(nextStepId, out var assignments))
            {
                assignmentModels.AddRange(assignments);
            }

            // Step 4.3: Compose and publish ExecuteActivityCommand
            var command = new ExecuteActivityCommand
            {
                ProcessorId = nextStepEntity.ProcessorId,
                OrchestratedFlowEntityId = activityEvent.OrchestratedFlowEntityId,
                StepId = nextStepId,
                ExecutionId = activityEvent.ExecutionId,
                Entities = assignmentModels,
                CorrelationId = activityEvent.CorrelationId
            };

            await _bus.Publish(command);

            _logger.LogDebug("Published ExecuteActivityCommand for next step. NextStepId: {NextStepId}, ProcessorId: {ProcessorId}, AssignmentCount: {AssignmentCount}",
                nextStepId, nextStepEntity.ProcessorId, assignmentModels.Count);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _logger.LogError(ex, "Failed to process next step. NextStepId: {NextStepId}", nextStepId);
            throw;
        }
    }

    /// <summary>
    /// Copies cache processor data from source processor to destination processor
    /// </summary>
    /// <param name="activityEvent">The activity executed event</param>
    /// <param name="nextStepId">The next step ID</param>
    /// <param name="destinationProcessorId">The destination processor ID</param>
    private async Task CopyCacheProcessorDataAsync(ActivityExecutedEvent activityEvent, Guid nextStepId, Guid destinationProcessorId)
    {
        using var activity = ActivitySource.StartActivity("CopyCacheProcessorData");
        activity?.SetTag("sourceProcessorId", activityEvent.ProcessorId.ToString())
                ?.SetTag("destinationProcessorId", destinationProcessorId.ToString())
                ?.SetTag("nextStepId", nextStepId.ToString());

        try
        {
            // Source cache location
            var sourceMapName = activityEvent.ProcessorId.ToString();
            var sourceKey = _rawCacheService.GetCacheKey(activityEvent.OrchestratedFlowEntityId, activityEvent.StepId, activityEvent.ExecutionId, activityEvent.CorrelationId);

            // Destination cache location
            var destinationMapName = destinationProcessorId.ToString();
            var destinationKey = _rawCacheService.GetCacheKey(activityEvent.OrchestratedFlowEntityId, nextStepId, activityEvent.ExecutionId, activityEvent.CorrelationId);

            // Copy data from source to destination
            var sourceData = await _rawCacheService.GetAsync(sourceMapName, sourceKey);
            if (!string.IsNullOrEmpty(sourceData))
            {
                await _rawCacheService.SetAsync(destinationMapName, destinationKey, sourceData);
                
                _logger.LogDebug("Copied cache processor data. Source: {SourceMapName}:{SourceKey} -> Destination: {DestinationMapName}:{DestinationKey}",
                    sourceMapName, sourceKey, destinationMapName, destinationKey);
            }
            else
            {
                _logger.LogWarning("No source data found to copy. Source: {SourceMapName}:{SourceKey}",
                    sourceMapName, sourceKey);
            }

            // Delete source cache data after successful copy
            await _rawCacheService.RemoveAsync(sourceMapName, sourceKey);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            _logger.LogError(ex, "Failed to copy cache processor data. SourceProcessorId: {SourceProcessorId}, DestinationProcessorId: {DestinationProcessorId}",
                activityEvent.ProcessorId, destinationProcessorId);
            throw;
        }
    }




}

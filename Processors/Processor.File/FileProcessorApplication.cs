using Shared.Models;
using Shared.Processor.Application;
using Shared.Processor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Shared.Configuration;

namespace Processor.File;

/// <summary>
/// Sample concrete implementation of BaseProcessorApplication
/// Demonstrates how to create a specific processor service
/// The base class now provides a complete default implementation that can be overridden if needed
/// </summary>
public class FileProcessorApplication : BaseProcessorApplication
{
    // Removed ConfigureServices override - logging is configured through appsettings.json
    // This eliminates any potential interference with MassTransit consumer registration

    /// <summary>
    /// Concrete implementation of the activity processing logic
    /// This is where the specific processor business logic is implemented
    /// Handles input parsing and validation internally
    /// </summary>
    protected override async Task<ProcessedActivityData> ProcessActivityDataAsync(
        Guid processorId,
        Guid orchestratedFlowEntityId,
        Guid stepId,
        Guid executionId,
        List<AssignmentModel> entities,
        string inputData,
        Guid correlationId ,
        CancellationToken cancellationToken = default)
    {
        // HASH VALIDATION TEST: This comment will change the implementation hash to demonstrate validation failure
        // Get logger from service provider
        var logger = ServiceProvider.GetRequiredService<ILogger<FileProcessorApplication>>();

        logger.LogInformation(
            "Processing activity. ProcessorId: {ProcessorId}, StepId: {StepId}, ExecutionId: {ExecutionId}, EntitiesCount: {EntitiesCount}",
            processorId, stepId, executionId, entities.Count);

        // 1. Parse input data (concrete processor responsibility)
        JsonElement inputDataElement;
        JsonElement? inputMetadata = null;

        try
        {
            if (string.IsNullOrEmpty(inputData))
            {
                // Create default empty structures for empty input
                var emptyJson = "{\"data\":{},\"metadata\":{}}";
                var inputObject = JsonSerializer.Deserialize<JsonElement>(emptyJson);
                inputDataElement = inputObject.GetProperty("data");
            }
            else
            {
                // Parse input data for normal case
                var inputObject = JsonSerializer.Deserialize<JsonElement>(inputData);
                inputDataElement = inputObject.TryGetProperty("message", out var messageElement) ? messageElement : inputObject;
                inputMetadata = inputObject.TryGetProperty("metadata", out var metadataElement) ? metadataElement : null;
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse input data: {ex.Message}");
        }

        // 2. Validate input data against InputSchema (concrete processor responsibility)
        if (!await ValidateInputDataAsync(inputData))
        {
            throw new InvalidOperationException("Input data validation failed against InputSchema");
        }

        // Simulate some processing time
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

        // Log input data details
        logger.LogInformation(
            "Input data type: {InputDataType}, Has metadata: {HasMetadata}",
            inputDataElement.ValueKind, inputMetadata.HasValue);

        // Process entities generically without specific type handling
        string sampleData = "No entities to process";
        if (entities.Any())
        {
            logger.LogInformation(
                "Processing {EntityCount} entities of types: {EntityTypes}",
                entities.Count,
                string.Join(", ", entities.Select(e => e.GetType().Name).Distinct()));

            // Use first entity for sample processing if available
            var firstEntity = entities.First();
            sampleData = $"Processed entity: {firstEntity.GetType().Name} (EntityId: {firstEntity.EntityId})";
        }

        // Log processing summary
        logger.LogInformation(
            "Completed processing entities. Total: {TotalEntities}, SampleData: {SampleData}",
            entities.Count, sampleData);

        // Return processed data
        return new ProcessedActivityData
        {
            Result = "File processing completed successfully",
            Status = "completed",
            Data = new
            {
                processorId = processorId.ToString(),
                orchestratedFlowEntityId = orchestratedFlowEntityId.ToString(),
                entitiesProcessed = entities.Count,
                processingDetails = new
                {
                    processedAt = DateTime.UtcNow,
                    processingDuration = "100ms",
                    inputDataReceived = !string.IsNullOrEmpty(inputData),
                    inputMetadataReceived = inputMetadata.HasValue,
                    sampleData = sampleData,
                    entityTypes = entities.Select(e => e.GetType().Name).Distinct().ToArray(),
                    entities = entities.Select(e => new
                    {
                        entityId = e.EntityId.ToString(),
                        type = e.GetType().Name,
                        assignmentType = e.GetType().Name
                    }).ToArray()
                }
            },
            ProcessorName = "EnhancedFileProcessor",
            Version = "3.0",
            ExecutionId = executionId == Guid.Empty ? Guid.NewGuid() : executionId
        };
    }

    /// <summary>
    /// Validates input data against the input schema
    /// </summary>
    /// <param name="inputData">Raw input data string to validate</param>
    /// <returns>True if validation passes, false otherwise</returns>
    private async Task<bool> ValidateInputDataAsync(string inputData)
    {
        try
        {
            var processorService = ServiceProvider.GetService<IProcessorService>();
            if (processorService == null)
            {
                // No processor service available - skip validation
                return true;
            }

            return await processorService.ValidateInputDataAsync(inputData);
        }
        catch (Exception ex)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<FileProcessorApplication>>();
            logger.LogError(ex, "Input schema validation failed with exception");
            return false;
        }
    }
}
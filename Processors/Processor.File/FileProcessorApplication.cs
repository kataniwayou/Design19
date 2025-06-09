using Shared.Entities.Base;
using Shared.Entities;
using Shared.Processor.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Processor.File;

/// <summary>
/// Sample concrete implementation of BaseProcessorApplication
/// Demonstrates how to create a specific processor service
/// The base class now provides a complete default implementation that can be overridden if needed
/// </summary>
public class FileProcessorApplication : BaseProcessorApplication
{


    /// <summary>
    /// Override to add console logging for debugging
    /// </summary>
    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Call base implementation FIRST to ensure MassTransit is configured properly
        base.ConfigureServices(services, configuration);

        // Then add our custom logging (this won't interfere with MassTransit)
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add logging to verify consumer registration
        Console.WriteLine("FileProcessor ConfigureServices completed - MassTransit consumer should be registered");
    }

    /// <summary>
    /// Concrete implementation of the activity processing logic
    /// This is where the specific processor business logic is implemented
    /// </summary>
    protected override async Task<ProcessedActivityData> ProcessActivityDataAsync(
        Guid processorId,
        Guid orchestratedFlowEntityId,
        Guid stepId,
        Guid executionId,
        List<AssignmentModel> entities,
        JsonElement inputData,
        JsonElement? inputMetadata,
        Guid correlationId ,
        CancellationToken cancellationToken = default)
    {
        // HASH VALIDATION TEST: This comment will change the implementation hash to demonstrate validation failure
        // Get logger from service provider
        var logger = ServiceProvider.GetRequiredService<ILogger<FileProcessorApplication>>();

        // Simulate some processing time
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

        // Process the data (simplified from original implementation)
        logger.LogInformation(
            "Sample processor processing data. ProcessorId: {ProcessorId}, StepId: {StepId}",
            processorId, stepId);

        // Initialize sample data for processing
        var sampleData = "This is sample data from the file processor";

        // Process entities generically without specific type handling
        if (entities.Any())
        {
            logger.LogInformation(
                "Processing {EntityCount} entities of types: {EntityTypes}",
                entities.Count,
                string.Join(", ", entities.Select(e => e.Type).Distinct()));

            // Use first entity for sample processing if available
            var firstEntity = entities.First();
            sampleData = $"Processed entity: {firstEntity.Type} (EntityId: {firstEntity.EntityId})";
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
                    inputDataReceived = true,
                    inputMetadataReceived = inputMetadata.HasValue,
                    sampleData = sampleData,
                    entityTypes = entities.Select(e => e.Type).Distinct().ToArray(),
                    entities = entities.Select(e => new
                    {
                        entityId = e.EntityId.ToString(),
                        type = e.Type,
                        assignmentType = e.GetType().Name
                    }).ToArray()
                }
            },
            ProcessorName = "EnhancedFileProcessor",
            Version = "3.0",
            ExecutionId = executionId == Guid.Empty ? Guid.NewGuid() : executionId
        };
    }
}
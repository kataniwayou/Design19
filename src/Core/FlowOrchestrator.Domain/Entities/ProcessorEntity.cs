using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;
using System.Text.Json.Serialization;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents a processor entity in the FlowOrchestrator system.
/// A processor entity represents a service that processes and transforms data.
/// </summary>
public class ProcessorEntity : AbstractEntity
{
    /// <summary>
    /// Gets or sets the type of the processor.
    /// </summary>
    public string ProcessorType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capabilities of the processor.
    /// </summary>
    public List<string> Capabilities { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the input schema of the processor.
    /// </summary>
    public SchemaDefinition InputSchema { get; set; } = new SchemaDefinition();

    /// <summary>
    /// Gets or sets the output schema of the processor.
    /// </summary>
    public SchemaDefinition OutputSchema { get; set; } = new SchemaDefinition();

    /// <summary>
    /// Gets or sets the configuration parameters of the processor.
    /// </summary>
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; } = new Dictionary<string, ParameterDefinition>();

    /// <summary>
    /// Gets or sets the status of the processor.
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the last known address of the processor service.
    /// </summary>
    public string LastKnownAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the processing logic type.
    /// </summary>
    public string ProcessingLogicType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the processing logic configuration.
    /// </summary>
    public Dictionary<string, object> ProcessingLogicConfiguration { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the performance metrics of the processor.
    /// </summary>
    public ProcessorPerformanceMetrics PerformanceMetrics { get; set; } = new ProcessorPerformanceMetrics();
}

/// <summary>
/// Represents processor performance metrics in the FlowOrchestrator system.
/// </summary>
public class ProcessorPerformanceMetrics
{
    /// <summary>
    /// Gets or sets the average processing time in milliseconds.
    /// </summary>
    public double AverageProcessingTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the maximum processing time in milliseconds.
    /// </summary>
    public double MaxProcessingTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the minimum processing time in milliseconds.
    /// </summary>
    public double MinProcessingTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the average throughput in items per second.
    /// </summary>
    public double AverageThroughputItemsPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the maximum throughput in items per second.
    /// </summary>
    public double MaxThroughputItemsPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the average memory usage in megabytes.
    /// </summary>
    public double AverageMemoryUsageMb { get; set; }

    /// <summary>
    /// Gets or sets the maximum memory usage in megabytes.
    /// </summary>
    public double MaxMemoryUsageMb { get; set; }

    /// <summary>
    /// Gets or sets the error rate as a percentage.
    /// </summary>
    public double ErrorRatePercentage { get; set; }

    /// <summary>
    /// Gets or sets the last updated timestamp.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

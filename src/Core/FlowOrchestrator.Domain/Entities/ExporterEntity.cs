using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;
using System.Text.Json.Serialization;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents an exporter entity in the FlowOrchestrator system.
/// An exporter entity represents a service that exports data to external destinations.
/// </summary>
public class ExporterEntity : AbstractEntity
{
    /// <summary>
    /// Gets or sets the type of the exporter.
    /// </summary>
    public string ExporterType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the protocol used by the exporter.
    /// </summary>
    public string Protocol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capabilities of the exporter.
    /// </summary>
    public List<string> Capabilities { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the input schema of the exporter.
    /// </summary>
    public SchemaDefinition InputSchema { get; set; } = new SchemaDefinition();

    /// <summary>
    /// Gets or sets the configuration parameters of the exporter.
    /// </summary>
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; } = new Dictionary<string, ParameterDefinition>();

    /// <summary>
    /// Gets or sets the protocol capabilities of the exporter.
    /// </summary>
    public ProtocolCapabilities ProtocolCapabilities { get; set; } = new ProtocolCapabilities();

    /// <summary>
    /// Gets or sets the status of the exporter.
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the last known address of the exporter service.
    /// </summary>
    public string LastKnownAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delivery guarantees of the exporter.
    /// </summary>
    public DeliveryGuarantees DeliveryGuarantees { get; set; } = new DeliveryGuarantees();

    /// <summary>
    /// Gets or sets the performance metrics of the exporter.
    /// </summary>
    public ExporterPerformanceMetrics PerformanceMetrics { get; set; } = new ExporterPerformanceMetrics();
}

/// <summary>
/// Represents delivery guarantees in the FlowOrchestrator system.
/// </summary>
public class DeliveryGuarantees
{
    /// <summary>
    /// Gets or sets a value indicating whether at-least-once delivery is supported.
    /// </summary>
    public bool SupportsAtLeastOnceDelivery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether exactly-once delivery is supported.
    /// </summary>
    public bool SupportsExactlyOnceDelivery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ordered delivery is supported.
    /// </summary>
    public bool SupportsOrderedDelivery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether transactions are supported.
    /// </summary>
    public bool SupportsTransactions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether retries are supported.
    /// </summary>
    public bool SupportsRetries { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of retries.
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// Gets or sets the retry interval in milliseconds.
    /// </summary>
    public int RetryIntervalMs { get; set; }
}

/// <summary>
/// Represents exporter performance metrics in the FlowOrchestrator system.
/// </summary>
public class ExporterPerformanceMetrics
{
    /// <summary>
    /// Gets or sets the average export time in milliseconds.
    /// </summary>
    public double AverageExportTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the maximum export time in milliseconds.
    /// </summary>
    public double MaxExportTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the minimum export time in milliseconds.
    /// </summary>
    public double MinExportTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the average throughput in items per second.
    /// </summary>
    public double AverageThroughputItemsPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the maximum throughput in items per second.
    /// </summary>
    public double MaxThroughputItemsPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the error rate as a percentage.
    /// </summary>
    public double ErrorRatePercentage { get; set; }

    /// <summary>
    /// Gets or sets the last updated timestamp.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

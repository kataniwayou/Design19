using Shared.Processor.Models;
using System.Text.Json.Serialization;

namespace Manager.Orchestrator.Models;

/// <summary>
/// Response model for processor health status
/// </summary>
public class ProcessorHealthResponse
{
    /// <summary>
    /// ID of the processor
    /// </summary>
    [JsonPropertyName("processorId")]
    public Guid ProcessorId { get; set; }

    /// <summary>
    /// Overall health status
    /// </summary>
    [JsonPropertyName("status")]
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Detailed health message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when health was last updated
    /// </summary>
    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Timestamp when this health entry expires
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// ID of the pod that reported this health status
    /// </summary>
    [JsonPropertyName("reportingPodId")]
    public string ReportingPodId { get; set; } = string.Empty;

    /// <summary>
    /// Processor uptime since last restart
    /// </summary>
    [JsonPropertyName("uptime")]
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// Processor metadata information
    /// </summary>
    [JsonPropertyName("metadata")]
    public ProcessorMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Performance metrics for the processor
    /// </summary>
    [JsonPropertyName("performanceMetrics")]
    public ProcessorPerformanceMetrics PerformanceMetrics { get; set; } = new();

    /// <summary>
    /// Detailed health check results
    /// </summary>
    [JsonPropertyName("healthChecks")]
    public Dictionary<string, HealthCheckResult> HealthChecks { get; set; } = new();

    /// <summary>
    /// Indicates if this health entry is still valid based on TTL
    /// </summary>
    [JsonPropertyName("isExpired")]
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>
    /// Timestamp when this response was generated
    /// </summary>
    [JsonPropertyName("retrievedAt")]
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Response model for multiple processors health status
/// </summary>
public class ProcessorsHealthResponse
{
    /// <summary>
    /// The orchestrated flow ID this health check is for
    /// </summary>
    [JsonPropertyName("orchestratedFlowId")]
    public Guid OrchestratedFlowId { get; set; }

    /// <summary>
    /// Dictionary of processor health statuses with processor ID as key
    /// </summary>
    [JsonPropertyName("processors")]
    public Dictionary<Guid, ProcessorHealthResponse> Processors { get; set; } = new();

    /// <summary>
    /// Summary of overall health status
    /// </summary>
    [JsonPropertyName("summary")]
    public ProcessorsHealthSummary Summary { get; set; } = new();

    /// <summary>
    /// Timestamp when this response was generated
    /// </summary>
    [JsonPropertyName("retrievedAt")]
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Summary of processors health status
/// </summary>
public class ProcessorsHealthSummary
{
    /// <summary>
    /// Total number of processors
    /// </summary>
    [JsonPropertyName("totalProcessors")]
    public int TotalProcessors { get; set; }

    /// <summary>
    /// Number of healthy processors
    /// </summary>
    [JsonPropertyName("healthyProcessors")]
    public int HealthyProcessors { get; set; }

    /// <summary>
    /// Number of degraded processors
    /// </summary>
    [JsonPropertyName("degradedProcessors")]
    public int DegradedProcessors { get; set; }

    /// <summary>
    /// Number of unhealthy processors
    /// </summary>
    [JsonPropertyName("unhealthyProcessors")]
    public int UnhealthyProcessors { get; set; }

    /// <summary>
    /// Number of processors with no health data
    /// </summary>
    [JsonPropertyName("noHealthDataProcessors")]
    public int NoHealthDataProcessors { get; set; }

    /// <summary>
    /// Overall health status based on all processors
    /// </summary>
    [JsonPropertyName("overallStatus")]
    public HealthStatus OverallStatus { get; set; }

    /// <summary>
    /// List of processor IDs that are unhealthy or have no health data
    /// </summary>
    [JsonPropertyName("problematicProcessors")]
    public List<Guid> ProblematicProcessors { get; set; } = new();
}

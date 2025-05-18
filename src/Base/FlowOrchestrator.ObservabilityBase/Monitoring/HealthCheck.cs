namespace FlowOrchestrator.ObservabilityBase.Monitoring;

/// <summary>
/// Health check.
/// </summary>
public class HealthCheck
{
    /// <summary>
    /// Gets or sets the health check name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the health check description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the health check function.
    /// </summary>
    public Func<Task<HealthCheckResult>> Check { get; set; } = () => Task.FromResult(new HealthCheckResult());

    /// <summary>
    /// Gets or sets the health check tags.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
}

/// <summary>
/// Health check result.
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Gets or sets the health check status.
    /// </summary>
    public HealthCheckStatus Status { get; set; } = HealthCheckStatus.Unknown;

    /// <summary>
    /// Gets or sets the health check description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the health check data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Health check results.
/// </summary>
public class HealthCheckResults
{
    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the overall status.
    /// </summary>
    public HealthCheckStatus Status { get; set; } = HealthCheckStatus.Unknown;

    /// <summary>
    /// Gets or sets the health check results.
    /// </summary>
    public Dictionary<string, HealthCheckResult> Results { get; set; } = new Dictionary<string, HealthCheckResult>();
}

/// <summary>
/// Health check status.
/// </summary>
public enum HealthCheckStatus
{
    /// <summary>
    /// The health check status is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The health check is healthy.
    /// </summary>
    Healthy = 1,

    /// <summary>
    /// The health check is degraded.
    /// </summary>
    Degraded = 2,

    /// <summary>
    /// The health check is unhealthy.
    /// </summary>
    Unhealthy = 3
}

/// <summary>
/// Health status.
/// </summary>
public class HealthStatus
{
    /// <summary>
    /// Gets or sets the health check name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the health check status.
    /// </summary>
    public HealthCheckStatus Status { get; set; } = HealthCheckStatus.Unknown;

    /// <summary>
    /// Gets or sets the last check timestamp.
    /// </summary>
    public DateTime LastCheckTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the last status change timestamp.
    /// </summary>
    public DateTime LastStatusChangeTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the last check duration.
    /// </summary>
    public TimeSpan LastCheckDuration { get; set; } = TimeSpan.Zero;
}

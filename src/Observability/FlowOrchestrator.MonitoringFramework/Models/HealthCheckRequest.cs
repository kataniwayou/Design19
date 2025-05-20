using FlowOrchestrator.ObservabilityBase.Monitoring;

namespace FlowOrchestrator.MonitoringFramework;

/// <summary>
/// Represents a health check request.
/// </summary>
public class HealthCheckRequest
{
    /// <summary>
    /// Gets or sets the component identifier.
    /// </summary>
    public string ComponentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check name.
    /// </summary>
    public string CheckName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public HealthCheckStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }
}

namespace FlowOrchestrator.MonitoringFramework;

/// <summary>
/// Represents a resource utilization request.
/// </summary>
public class ResourceUtilizationRequest
{
    /// <summary>
    /// Gets or sets the component identifier.
    /// </summary>
    public string ComponentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource type.
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the attributes.
    /// </summary>
    public Dictionary<string, object>? Attributes { get; set; }
}

using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Represents a component status message.
/// </summary>
public class ComponentStatusMessage : IMessage
{
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the component identifier.
    /// </summary>
    public string ComponentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the component name.
    /// </summary>
    public string ComponentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the component type.
    /// </summary>
    public string ComponentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

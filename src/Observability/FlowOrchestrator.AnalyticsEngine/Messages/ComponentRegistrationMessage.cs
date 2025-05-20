using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.Abstractions.Messages;

/// <summary>
/// Represents a component registration message.
/// </summary>
public class ComponentRegistrationMessage : IMessage
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
}

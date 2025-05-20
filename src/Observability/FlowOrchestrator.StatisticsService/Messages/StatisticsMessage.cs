using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.StatisticsService;

/// <summary>
/// Represents a statistics message.
/// </summary>
public class StatisticsMessage : IMessage
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
    /// Gets or sets the category.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public StatisticsType Type { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the attributes.
    /// </summary>
    public Dictionary<string, object>? Attributes { get; set; }
}

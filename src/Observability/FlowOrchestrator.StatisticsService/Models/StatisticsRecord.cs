using MongoDB.Bson.Serialization.Attributes;

namespace FlowOrchestrator.StatisticsService;

/// <summary>
/// Represents a statistics record.
/// </summary>
public class StatisticsRecord
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [BsonId]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

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

/// <summary>
/// Represents the type of statistics.
/// </summary>
public enum StatisticsType
{
    /// <summary>
    /// Counter type.
    /// </summary>
    Counter,

    /// <summary>
    /// Gauge type.
    /// </summary>
    Gauge,

    /// <summary>
    /// Histogram type.
    /// </summary>
    Histogram
}

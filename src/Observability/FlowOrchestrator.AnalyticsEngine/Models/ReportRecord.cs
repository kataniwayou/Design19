using MongoDB.Bson.Serialization.Attributes;

namespace FlowOrchestrator.AnalyticsEngine;

/// <summary>
/// Represents a report record.
/// </summary>
public class ReportRecord
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
    /// Gets or sets the report type.
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parameters.
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}

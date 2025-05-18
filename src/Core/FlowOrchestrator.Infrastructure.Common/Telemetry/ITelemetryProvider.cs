namespace FlowOrchestrator.Infrastructure.Common.Telemetry;

/// <summary>
/// Defines the contract for a telemetry provider.
/// </summary>
public interface ITelemetryProvider
{
    /// <summary>
    /// Creates a span.
    /// </summary>
    /// <param name="name">The name of the span.</param>
    /// <returns>The span.</returns>
    ISpan CreateSpan(string name);

    /// <summary>
    /// Records a metric.
    /// </summary>
    /// <param name="name">The name of the metric.</param>
    /// <param name="value">The value of the metric.</param>
    /// <param name="tags">The tags associated with the metric.</param>
    void RecordMetric(string name, double value, Dictionary<string, object>? tags = null);

    /// <summary>
    /// Records an event.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="attributes">The attributes associated with the event.</param>
    void RecordEvent(string name, Dictionary<string, object>? attributes = null);
}

/// <summary>
/// Defines the contract for a span.
/// </summary>
public interface ISpan : IDisposable
{
    /// <summary>
    /// Sets an attribute on the span.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns>The span.</returns>
    ISpan SetAttribute(string key, string value);

    /// <summary>
    /// Sets an attribute on the span.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns>The span.</returns>
    ISpan SetAttribute(string key, int value);

    /// <summary>
    /// Sets an attribute on the span.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns>The span.</returns>
    ISpan SetAttribute(string key, double value);

    /// <summary>
    /// Sets an attribute on the span.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <param name="value">The attribute value.</param>
    /// <returns>The span.</returns>
    ISpan SetAttribute(string key, bool value);

    /// <summary>
    /// Adds an event to the span.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="attributes">The attributes associated with the event.</param>
    /// <returns>The span.</returns>
    ISpan AddEvent(string name, Dictionary<string, object>? attributes = null);

    /// <summary>
    /// Sets the status of the span.
    /// </summary>
    /// <param name="status">The status.</param>
    /// <param name="description">The description.</param>
    /// <returns>The span.</returns>
    ISpan SetStatus(SpanStatus status, string? description = null);

    /// <summary>
    /// Records an exception.
    /// </summary>
    /// <param name="exception">The exception to record.</param>
    /// <returns>The span.</returns>
    ISpan RecordException(Exception exception);
}

/// <summary>
/// Defines the status of a span.
/// </summary>
public enum SpanStatus
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Ok,

    /// <summary>
    /// The operation completed with an error.
    /// </summary>
    Error,

    /// <summary>
    /// The operation was cancelled.
    /// </summary>
    Cancelled
}

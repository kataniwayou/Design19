using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace FlowOrchestrator.Infrastructure.Common.Telemetry;

/// <summary>
/// OpenTelemetry implementation of the telemetry provider.
/// </summary>
public class OpenTelemetryProvider : ITelemetryProvider
{
    private readonly Tracer _tracer;
    private readonly string _serviceName;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetryProvider"/> class.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    public OpenTelemetryProvider(string serviceName)
    {
        _serviceName = serviceName;
        _tracer = TracerProvider.Default.GetTracer(serviceName);
    }

    /// <summary>
    /// Creates a span.
    /// </summary>
    /// <param name="name">The name of the span.</param>
    /// <returns>The span.</returns>
    public ISpan CreateSpan(string name)
    {
        var span = _tracer.StartSpan(name);
        span.SetAttribute("service.name", _serviceName);
        return new OpenTelemetrySpan(span);
    }

    /// <summary>
    /// Records a metric.
    /// </summary>
    /// <param name="name">The name of the metric.</param>
    /// <param name="value">The value of the metric.</param>
    /// <param name="tags">The tags associated with the metric.</param>
    public void RecordMetric(string name, double value, Dictionary<string, object>? tags = null)
    {
        // OpenTelemetry metrics are not yet implemented in this version
        // This is a placeholder for future implementation
    }

    /// <summary>
    /// Records an event.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="attributes">The attributes associated with the event.</param>
    public void RecordEvent(string name, Dictionary<string, object>? attributes = null)
    {
        var span = Tracer.CurrentSpan;
        if (span != null)
        {
            if (attributes != null)
            {
                // Convert Dictionary to SpanAttributes
                var spanAttributes = new SpanAttributes();
                foreach (var attribute in attributes)
                {
                    spanAttributes.Add(attribute.Key, attribute.Value.ToString());
                }
                span.AddEvent(name, spanAttributes);
            }
            else
            {
                span.AddEvent(name);
            }
        }
    }

    /// <summary>
    /// OpenTelemetry implementation of the span.
    /// </summary>
    private class OpenTelemetrySpan : ISpan
    {
        private readonly TelemetrySpan _span;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTelemetrySpan"/> class.
        /// </summary>
        /// <param name="span">The OpenTelemetry span.</param>
        public OpenTelemetrySpan(TelemetrySpan span)
        {
            _span = span;
        }

        /// <summary>
        /// Sets an attribute on the span.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The span.</returns>
        public ISpan SetAttribute(string key, string value)
        {
            _span.SetAttribute(key, value);
            return this;
        }

        /// <summary>
        /// Sets an attribute on the span.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The span.</returns>
        public ISpan SetAttribute(string key, int value)
        {
            _span.SetAttribute(key, value);
            return this;
        }

        /// <summary>
        /// Sets an attribute on the span.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The span.</returns>
        public ISpan SetAttribute(string key, double value)
        {
            _span.SetAttribute(key, value);
            return this;
        }

        /// <summary>
        /// Sets an attribute on the span.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The span.</returns>
        public ISpan SetAttribute(string key, bool value)
        {
            _span.SetAttribute(key, value);
            return this;
        }

        /// <summary>
        /// Adds an event to the span.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="attributes">The attributes associated with the event.</param>
        /// <returns>The span.</returns>
        public ISpan AddEvent(string name, Dictionary<string, object>? attributes = null)
        {
            if (attributes != null)
            {
                // Convert Dictionary to SpanAttributes
                var spanAttributes = new SpanAttributes();
                foreach (var attribute in attributes)
                {
                    spanAttributes.Add(attribute.Key, attribute.Value.ToString());
                }
                _span.AddEvent(name, spanAttributes);
            }
            else
            {
                _span.AddEvent(name);
            }
            return this;
        }

        /// <summary>
        /// Sets the status of the span.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="description">The description.</param>
        /// <returns>The span.</returns>
        public ISpan SetStatus(SpanStatus status, string? description = null)
        {
            switch (status)
            {
                case SpanStatus.Ok:
                    _span.SetStatus(Status.Ok);
                    break;
                case SpanStatus.Error:
                    _span.SetStatus(Status.Error.WithDescription(description));
                    break;
                case SpanStatus.Cancelled:
                    _span.SetStatus(Status.Error.WithDescription("Cancelled"));
                    break;
            }
            return this;
        }

        /// <summary>
        /// Records an exception.
        /// </summary>
        /// <param name="exception">The exception to record.</param>
        /// <returns>The span.</returns>
        public ISpan RecordException(Exception exception)
        {
            _span.RecordException(exception);
            return this;
        }

        /// <summary>
        /// Disposes the span.
        /// </summary>
        public void Dispose()
        {
            _span.Dispose();
        }
    }
}

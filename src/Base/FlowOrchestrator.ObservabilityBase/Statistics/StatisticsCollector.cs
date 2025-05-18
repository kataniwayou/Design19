using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;

namespace FlowOrchestrator.ObservabilityBase.Statistics;

/// <summary>
/// Statistics collector.
/// </summary>
public class StatisticsCollector
{
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly Dictionary<string, StatisticsCounter> _counters = new Dictionary<string, StatisticsCounter>();
    private readonly Dictionary<string, StatisticsGauge> _gauges = new Dictionary<string, StatisticsGauge>();
    private readonly Dictionary<string, StatisticsHistogram> _histograms = new Dictionary<string, StatisticsHistogram>();

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsCollector"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public StatisticsCollector(
        ILogger logger,
        ITelemetryProvider telemetryProvider)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Increments a counter.
    /// </summary>
    /// <param name="name">The counter name.</param>
    /// <param name="value">The value to increment by.</param>
    /// <param name="attributes">The attributes.</param>
    public void IncrementCounter(string name, long value = 1, Dictionary<string, object>? attributes = null)
    {
        try
        {
            if (!_counters.TryGetValue(name, out var counter))
            {
                counter = new StatisticsCounter(name);
                _counters[name] = counter;
            }

            counter.Increment(value);
            _telemetryProvider.RecordMetric(name, value, attributes ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to increment counter {name}", ex);
        }
    }

    /// <summary>
    /// Sets a gauge.
    /// </summary>
    /// <param name="name">The gauge name.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="attributes">The attributes.</param>
    public void SetGauge(string name, double value, Dictionary<string, object>? attributes = null)
    {
        try
        {
            if (!_gauges.TryGetValue(name, out var gauge))
            {
                gauge = new StatisticsGauge(name);
                _gauges[name] = gauge;
            }

            gauge.Set(value);
            _telemetryProvider.RecordMetric(name, value, attributes ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to set gauge {name}", ex);
        }
    }

    /// <summary>
    /// Records a histogram value.
    /// </summary>
    /// <param name="name">The histogram name.</param>
    /// <param name="value">The value to record.</param>
    /// <param name="attributes">The attributes.</param>
    public void RecordHistogram(string name, double value, Dictionary<string, object>? attributes = null)
    {
        try
        {
            if (!_histograms.TryGetValue(name, out var histogram))
            {
                histogram = new StatisticsHistogram(name);
                _histograms[name] = histogram;
            }

            histogram.Record(value);
            _telemetryProvider.RecordMetric(name, value, attributes ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to record histogram {name}", ex);
        }
    }

    /// <summary>
    /// Gets all counters.
    /// </summary>
    /// <returns>The counters.</returns>
    public IReadOnlyDictionary<string, StatisticsCounter> GetCounters()
    {
        return _counters;
    }

    /// <summary>
    /// Gets all gauges.
    /// </summary>
    /// <returns>The gauges.</returns>
    public IReadOnlyDictionary<string, StatisticsGauge> GetGauges()
    {
        return _gauges;
    }

    /// <summary>
    /// Gets all histograms.
    /// </summary>
    /// <returns>The histograms.</returns>
    public IReadOnlyDictionary<string, StatisticsHistogram> GetHistograms()
    {
        return _histograms;
    }

    /// <summary>
    /// Gets a counter.
    /// </summary>
    /// <param name="name">The counter name.</param>
    /// <returns>The counter, or null if not found.</returns>
    public StatisticsCounter? GetCounter(string name)
    {
        return _counters.TryGetValue(name, out var counter) ? counter : null;
    }

    /// <summary>
    /// Gets a gauge.
    /// </summary>
    /// <param name="name">The gauge name.</param>
    /// <returns>The gauge, or null if not found.</returns>
    public StatisticsGauge? GetGauge(string name)
    {
        return _gauges.TryGetValue(name, out var gauge) ? gauge : null;
    }

    /// <summary>
    /// Gets a histogram.
    /// </summary>
    /// <param name="name">The histogram name.</param>
    /// <returns>The histogram, or null if not found.</returns>
    public StatisticsHistogram? GetHistogram(string name)
    {
        return _histograms.TryGetValue(name, out var histogram) ? histogram : null;
    }

    /// <summary>
    /// Resets all statistics.
    /// </summary>
    public void Reset()
    {
        _counters.Clear();
        _gauges.Clear();
        _histograms.Clear();
    }
}

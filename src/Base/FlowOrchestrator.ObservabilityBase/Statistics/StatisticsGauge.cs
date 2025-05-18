namespace FlowOrchestrator.ObservabilityBase.Statistics;

/// <summary>
/// Statistics gauge.
/// </summary>
public class StatisticsGauge
{
    private double _value;

    /// <summary>
    /// Gets the gauge name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the gauge value.
    /// </summary>
    public double Value => _value;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTime CreationTimestamp { get; }

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTime LastUpdateTimestamp { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsGauge"/> class.
    /// </summary>
    /// <param name="name">The gauge name.</param>
    public StatisticsGauge(string name)
    {
        Name = name;
        _value = 0;
        CreationTimestamp = DateTime.UtcNow;
        LastUpdateTimestamp = CreationTimestamp;
    }

    /// <summary>
    /// Sets the gauge value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Set(double value)
    {
        Interlocked.Exchange(ref _value, value);
        LastUpdateTimestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets the gauge.
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref _value, 0);
        LastUpdateTimestamp = DateTime.UtcNow;
    }
}

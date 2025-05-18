namespace FlowOrchestrator.ObservabilityBase.Statistics;

/// <summary>
/// Statistics counter.
/// </summary>
public class StatisticsCounter
{
    private long _value;

    /// <summary>
    /// Gets the counter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the counter value.
    /// </summary>
    public long Value => _value;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTime CreationTimestamp { get; }

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTime LastUpdateTimestamp { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsCounter"/> class.
    /// </summary>
    /// <param name="name">The counter name.</param>
    public StatisticsCounter(string name)
    {
        Name = name;
        _value = 0;
        CreationTimestamp = DateTime.UtcNow;
        LastUpdateTimestamp = CreationTimestamp;
    }

    /// <summary>
    /// Increments the counter.
    /// </summary>
    /// <param name="value">The value to increment by.</param>
    public void Increment(long value = 1)
    {
        Interlocked.Add(ref _value, value);
        LastUpdateTimestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets the counter.
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref _value, 0);
        LastUpdateTimestamp = DateTime.UtcNow;
    }
}

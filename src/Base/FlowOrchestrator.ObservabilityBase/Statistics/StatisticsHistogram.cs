namespace FlowOrchestrator.ObservabilityBase.Statistics;

/// <summary>
/// Statistics histogram.
/// </summary>
public class StatisticsHistogram
{
    private readonly object _lock = new object();
    private readonly List<double> _values = new List<double>();

    /// <summary>
    /// Gets the histogram name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the histogram values.
    /// </summary>
    public IReadOnlyList<double> Values
    {
        get
        {
            lock (_lock)
            {
                return _values.ToList();
            }
        }
    }

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTime CreationTimestamp { get; }

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTime LastUpdateTimestamp { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsHistogram"/> class.
    /// </summary>
    /// <param name="name">The histogram name.</param>
    public StatisticsHistogram(string name)
    {
        Name = name;
        CreationTimestamp = DateTime.UtcNow;
        LastUpdateTimestamp = CreationTimestamp;
    }

    /// <summary>
    /// Records a value.
    /// </summary>
    /// <param name="value">The value to record.</param>
    public void Record(double value)
    {
        lock (_lock)
        {
            _values.Add(value);
            LastUpdateTimestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Gets the count of values.
    /// </summary>
    /// <returns>The count.</returns>
    public int Count()
    {
        lock (_lock)
        {
            return _values.Count;
        }
    }

    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    /// <returns>The minimum value, or 0 if no values.</returns>
    public double Min()
    {
        lock (_lock)
        {
            return _values.Count > 0 ? _values.Min() : 0;
        }
    }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    /// <returns>The maximum value, or 0 if no values.</returns>
    public double Max()
    {
        lock (_lock)
        {
            return _values.Count > 0 ? _values.Max() : 0;
        }
    }

    /// <summary>
    /// Gets the average value.
    /// </summary>
    /// <returns>The average value, or 0 if no values.</returns>
    public double Average()
    {
        lock (_lock)
        {
            return _values.Count > 0 ? _values.Average() : 0;
        }
    }

    /// <summary>
    /// Gets the sum of values.
    /// </summary>
    /// <returns>The sum, or 0 if no values.</returns>
    public double Sum()
    {
        lock (_lock)
        {
            return _values.Count > 0 ? _values.Sum() : 0;
        }
    }

    /// <summary>
    /// Gets the percentile value.
    /// </summary>
    /// <param name="percentile">The percentile (0-100).</param>
    /// <returns>The percentile value, or 0 if no values.</returns>
    public double Percentile(double percentile)
    {
        lock (_lock)
        {
            if (_values.Count == 0)
            {
                return 0;
            }

            var sortedValues = _values.OrderBy(v => v).ToList();
            var index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Count) - 1;
            return sortedValues[Math.Max(0, index)];
        }
    }

    /// <summary>
    /// Resets the histogram.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            _values.Clear();
            LastUpdateTimestamp = DateTime.UtcNow;
        }
    }
}

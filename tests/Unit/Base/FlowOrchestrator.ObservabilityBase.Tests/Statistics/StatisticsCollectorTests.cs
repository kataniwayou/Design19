using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ObservabilityBase.Statistics;
using Moq;
using Xunit;

namespace FlowOrchestrator.ObservabilityBase.Tests.Statistics;

public class StatisticsCollectorTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<ITelemetryProvider> _telemetryProviderMock;
    private readonly StatisticsCollector _statisticsCollector;

    public StatisticsCollectorTests()
    {
        _loggerMock = new Mock<ILogger>();
        _telemetryProviderMock = new Mock<ITelemetryProvider>();
        _statisticsCollector = new StatisticsCollector(_loggerMock.Object, _telemetryProviderMock.Object);
    }

    [Fact]
    public void IncrementCounter_ShouldIncrementValue()
    {
        // Arrange
        var counterName = "test_counter";
        var value = 5;

        // Act
        _statisticsCollector.IncrementCounter(counterName, value);
        var counter = _statisticsCollector.GetCounter(counterName);

        // Assert
        Assert.NotNull(counter);
        Assert.Equal(counterName, counter.Name);
        Assert.Equal(value, counter.Value);
        _telemetryProviderMock.Verify(tp => tp.RecordMetric(counterName, value, It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public void IncrementCounter_MultipleTimes_ShouldAccumulateValues()
    {
        // Arrange
        var counterName = "test_counter";

        // Act
        _statisticsCollector.IncrementCounter(counterName, 3);
        _statisticsCollector.IncrementCounter(counterName, 2);
        var counter = _statisticsCollector.GetCounter(counterName);

        // Assert
        Assert.NotNull(counter);
        Assert.Equal(5, counter.Value);
    }

    [Fact]
    public void SetGauge_ShouldSetValue()
    {
        // Arrange
        var gaugeName = "test_gauge";
        var value = 3.14;

        // Act
        _statisticsCollector.SetGauge(gaugeName, value);
        var gauge = _statisticsCollector.GetGauge(gaugeName);

        // Assert
        Assert.NotNull(gauge);
        Assert.Equal(gaugeName, gauge.Name);
        Assert.Equal(value, gauge.Value);
        _telemetryProviderMock.Verify(tp => tp.RecordMetric(gaugeName, value, It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public void SetGauge_MultipleTimes_ShouldOverwriteValue()
    {
        // Arrange
        var gaugeName = "test_gauge";

        // Act
        _statisticsCollector.SetGauge(gaugeName, 3.14);
        _statisticsCollector.SetGauge(gaugeName, 2.71);
        var gauge = _statisticsCollector.GetGauge(gaugeName);

        // Assert
        Assert.NotNull(gauge);
        Assert.Equal(2.71, gauge.Value);
    }

    [Fact]
    public void RecordHistogram_ShouldRecordValue()
    {
        // Arrange
        var histogramName = "test_histogram";
        var value = 42.0;

        // Act
        _statisticsCollector.RecordHistogram(histogramName, value);
        var histogram = _statisticsCollector.GetHistogram(histogramName);

        // Assert
        Assert.NotNull(histogram);
        Assert.Equal(histogramName, histogram.Name);
        Assert.Single(histogram.Values);
        Assert.Equal(value, histogram.Values[0]);
        _telemetryProviderMock.Verify(tp => tp.RecordMetric(histogramName, value, It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public void RecordHistogram_MultipleTimes_ShouldAccumulateValues()
    {
        // Arrange
        var histogramName = "test_histogram";

        // Act
        _statisticsCollector.RecordHistogram(histogramName, 1.0);
        _statisticsCollector.RecordHistogram(histogramName, 2.0);
        _statisticsCollector.RecordHistogram(histogramName, 3.0);
        var histogram = _statisticsCollector.GetHistogram(histogramName);

        // Assert
        Assert.NotNull(histogram);
        Assert.Equal(3, histogram.Values.Count);
        Assert.Equal(1.0, histogram.Values[0]);
        Assert.Equal(2.0, histogram.Values[1]);
        Assert.Equal(3.0, histogram.Values[2]);
    }

    [Fact]
    public void Reset_ShouldClearAllStatistics()
    {
        // Arrange
        _statisticsCollector.IncrementCounter("counter1");
        _statisticsCollector.SetGauge("gauge1", 1.0);
        _statisticsCollector.RecordHistogram("histogram1", 1.0);

        // Act
        _statisticsCollector.Reset();

        // Assert
        Assert.Empty(_statisticsCollector.GetCounters());
        Assert.Empty(_statisticsCollector.GetGauges());
        Assert.Empty(_statisticsCollector.GetHistograms());
    }

    [Fact]
    public void GetCounter_NonExistentCounter_ShouldReturnNull()
    {
        // Act
        var counter = _statisticsCollector.GetCounter("non_existent");

        // Assert
        Assert.Null(counter);
    }

    [Fact]
    public void GetGauge_NonExistentGauge_ShouldReturnNull()
    {
        // Act
        var gauge = _statisticsCollector.GetGauge("non_existent");

        // Assert
        Assert.Null(gauge);
    }

    [Fact]
    public void GetHistogram_NonExistentHistogram_ShouldReturnNull()
    {
        // Act
        var histogram = _statisticsCollector.GetHistogram("non_existent");

        // Assert
        Assert.Null(histogram);
    }
}

using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ObservabilityBase.Components;
using FlowOrchestrator.ObservabilityBase.Statistics;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace FlowOrchestrator.StatisticsService.Services;

/// <summary>
/// Service for collecting and managing statistics.
/// </summary>
public class StatisticsService : AbstractObservabilityComponent
{
    private readonly IMongoCollection<StatisticsRecord> _statisticsCollection;
    private readonly ConcurrentDictionary<string, StatisticsCounter> _counters = new();
    private readonly ConcurrentDictionary<string, StatisticsGauge> _gauges = new();
    private readonly ConcurrentDictionary<string, StatisticsHistogram> _histograms = new();
    private readonly StatisticsCollector _statisticsCollector;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="mongoClient">The MongoDB client.</param>
    public StatisticsService(
        FlowOrchestrator.Common.Logging.ILogger logger,
        Common.Configuration.IConfigurationProvider configurationProvider,
        Infrastructure.Common.Messaging.IMessageBus messageBus,
        ITelemetryProvider telemetryProvider,
        IMongoClient mongoClient)
        : base(logger, configurationProvider, messageBus, telemetryProvider)
    {
        var database = mongoClient.GetDatabase("FlowOrchestrator");
        _statisticsCollection = database.GetCollection<StatisticsRecord>("Statistics");
        _statisticsCollector = new StatisticsCollector(logger, telemetryProvider);
    }

    /// <inheritdoc/>
    public override string ComponentId => "StatisticsService";

    /// <inheritdoc/>
    public override string ComponentName => "Statistics Service";

    /// <inheritdoc/>
    public override string ComponentType => "StatisticsService";

    /// <inheritdoc/>
    public override string Version => "1.0.0";

    /// <summary>
    /// Initializes the service.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Create indexes
        var indexKeysDefinition = Builders<StatisticsRecord>.IndexKeys
            .Ascending(x => x.Timestamp)
            .Ascending(x => x.Category)
            .Ascending(x => x.Name);

        await _statisticsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<StatisticsRecord>(indexKeysDefinition));

        Logger.Info("Statistics service initialized");
    }

    /// <summary>
    /// Records a counter metric.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordCounterAsync(string category, string name, long value, Dictionary<string, object>? attributes = null)
    {
        using var span = TelemetryProvider.CreateSpan("StatisticsService.RecordCounter");
        span.SetAttribute("counter.category", category);
        span.SetAttribute("counter.name", name);
        span.SetAttribute("counter.value", value);

        try
        {
            // Record in memory
            _statisticsCollector.IncrementCounter(name, value, attributes);

            // Record in database
            var record = new StatisticsRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Category = category,
                Name = name,
                Type = StatisticsType.Counter,
                Value = value,
                Attributes = attributes
            };

            await _statisticsCollection.InsertOneAsync(record);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to record counter {category}.{name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Records a gauge metric.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordGaugeAsync(string category, string name, double value, Dictionary<string, object>? attributes = null)
    {
        using var span = TelemetryProvider.CreateSpan("StatisticsService.RecordGauge");
        span.SetAttribute("gauge.category", category);
        span.SetAttribute("gauge.name", name);
        span.SetAttribute("gauge.value", value);

        try
        {
            // Record in memory
            _statisticsCollector.SetGauge(name, value, attributes);

            // Record in database
            var record = new StatisticsRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Category = category,
                Name = name,
                Type = StatisticsType.Gauge,
                Value = value,
                Attributes = attributes
            };

            await _statisticsCollection.InsertOneAsync(record);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to record gauge {category}.{name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Records a histogram metric.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordHistogramAsync(string category, string name, double value, Dictionary<string, object>? attributes = null)
    {
        using var span = TelemetryProvider.CreateSpan("StatisticsService.RecordHistogram");
        span.SetAttribute("histogram.category", category);
        span.SetAttribute("histogram.name", name);
        span.SetAttribute("histogram.value", value);

        try
        {
            // Record in memory
            _statisticsCollector.RecordHistogram(name, value, attributes);

            // Record in database
            var record = new StatisticsRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Category = category,
                Name = name,
                Type = StatisticsType.Histogram,
                Value = value,
                Attributes = attributes
            };

            await _statisticsCollection.InsertOneAsync(record);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to record histogram {category}.{name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    protected override Abstractions.Messages.IMessage CreateRegistrationMessage()
    {
        return new Abstractions.Messages.ComponentRegistrationMessage
        {
            ComponentId = ComponentId,
            ComponentName = ComponentName,
            ComponentType = ComponentType,
            Version = Version,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <inheritdoc/>
    protected override Abstractions.Messages.IMessage CreateStatusMessage(string status, string message, string? correlationId = null)
    {
        return new Abstractions.Messages.ComponentStatusMessage
        {
            ComponentId = ComponentId,
            ComponentName = ComponentName,
            ComponentType = ComponentType,
            Version = Version,
            Status = status,
            Message = message,
            CorrelationId = correlationId ?? string.Empty,
            Timestamp = DateTime.UtcNow
        };
    }
}

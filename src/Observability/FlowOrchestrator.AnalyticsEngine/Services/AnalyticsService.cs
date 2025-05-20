using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ObservabilityBase.Components;
using MongoDB.Driver;

namespace FlowOrchestrator.AnalyticsEngine.Services;

/// <summary>
/// Service for analytics processing.
/// </summary>
public class AnalyticsService : AbstractObservabilityComponent
{
    private readonly IMongoCollection<AnalyticsRecord> _analyticsCollection;
    private readonly IMongoCollection<ReportRecord> _reportCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="mongoClient">The MongoDB client.</param>
    public AnalyticsService(
        FlowOrchestrator.Common.Logging.ILogger logger,
        Common.Configuration.IConfigurationProvider configurationProvider,
        Infrastructure.Common.Messaging.IMessageBus messageBus,
        ITelemetryProvider telemetryProvider,
        IMongoClient mongoClient)
        : base(logger, configurationProvider, messageBus, telemetryProvider)
    {
        var database = mongoClient.GetDatabase("FlowOrchestrator");
        _analyticsCollection = database.GetCollection<AnalyticsRecord>("Analytics");
        _reportCollection = database.GetCollection<ReportRecord>("Reports");
    }

    /// <inheritdoc/>
    public override string ComponentId => "AnalyticsService";

    /// <inheritdoc/>
    public override string ComponentName => "Analytics Service";

    /// <inheritdoc/>
    public override string ComponentType => "AnalyticsService";

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
        var analyticsIndexKeysDefinition = Builders<AnalyticsRecord>.IndexKeys
            .Ascending(x => x.Timestamp)
            .Ascending(x => x.Category)
            .Ascending(x => x.Name);

        await _analyticsCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<AnalyticsRecord>(analyticsIndexKeysDefinition));

        var reportIndexKeysDefinition = Builders<ReportRecord>.IndexKeys
            .Ascending(x => x.Timestamp)
            .Ascending(x => x.ReportType);

        await _reportCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<ReportRecord>(reportIndexKeysDefinition));

        Logger.Info("Analytics service initialized");
    }

    /// <summary>
    /// Records analytics data.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordAnalyticsDataAsync(
        string category,
        string name,
        double value,
        Dictionary<string, object>? attributes = null)
    {
        using var span = TelemetryProvider.CreateSpan("AnalyticsService.RecordAnalyticsData");
        span.SetAttribute("analytics.category", category);
        span.SetAttribute("analytics.name", name);
        span.SetAttribute("analytics.value", value);

        try
        {
            // Record in database
            var record = new AnalyticsRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Category = category,
                Name = name,
                Value = value,
                Attributes = attributes
            };

            await _analyticsCollection.InsertOneAsync(record);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to record analytics data {category}.{name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Generates a report.
    /// </summary>
    /// <param name="reportType">The report type.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The report data.</returns>
    public async Task<ReportRecord> GenerateReportAsync(
        string reportType,
        Dictionary<string, object>? parameters = null)
    {
        using var span = TelemetryProvider.CreateSpan("AnalyticsService.GenerateReport");
        span.SetAttribute("report.type", reportType);

        try
        {
            Logger.Info($"Generating report: {reportType}");

            // Generate report data
            var reportData = await GenerateReportDataAsync(reportType, parameters);

            // Create report record
            var report = new ReportRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                ReportType = reportType,
                Parameters = parameters,
                Data = reportData
            };

            // Save report
            await _reportCollection.InsertOneAsync(report);

            Logger.Info($"Report generated: {reportType}");
            span.SetStatus(SpanStatus.Ok);
            return report;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to generate report: {reportType}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Generates report data.
    /// </summary>
    /// <param name="reportType">The report type.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The report data.</returns>
    private async Task<Dictionary<string, object>> GenerateReportDataAsync(
        string reportType,
        Dictionary<string, object>? parameters = null)
    {
        // This is a placeholder for the actual report generation logic
        // In a real implementation, this would query the analytics data and generate the report
        return new Dictionary<string, object>
        {
            { "reportType", reportType },
            { "generatedAt", DateTime.UtcNow },
            { "sampleData", new[] { 1, 2, 3, 4, 5 } }
        };
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

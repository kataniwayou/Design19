using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ObservabilityBase.Components;
using FlowOrchestrator.ObservabilityBase.Monitoring;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace FlowOrchestrator.AlertingSystem.Services;

/// <summary>
/// Service for managing alerts.
/// </summary>
public class AlertingService : AbstractObservabilityComponent
{
    private readonly IMongoCollection<AlertRecord> _alertCollection;
    private readonly ConcurrentDictionary<string, AlertRule> _alertRules = new();
    private readonly ConcurrentDictionary<string, Alert> _activeAlerts = new();
    private readonly AlertManager _alertManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertingService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="mongoClient">The MongoDB client.</param>
    public AlertingService(
        FlowOrchestrator.Common.Logging.ILogger logger,
        Common.Configuration.IConfigurationProvider configurationProvider,
        Infrastructure.Common.Messaging.IMessageBus messageBus,
        ITelemetryProvider telemetryProvider,
        IMongoClient mongoClient)
        : base(logger, configurationProvider, messageBus, telemetryProvider)
    {
        var database = mongoClient.GetDatabase("FlowOrchestrator");
        _alertCollection = database.GetCollection<AlertRecord>("Alerts");
        _alertManager = new AlertManager(logger, telemetryProvider, messageBus);
    }

    /// <inheritdoc/>
    public override string ComponentId => "AlertingService";

    /// <inheritdoc/>
    public override string ComponentName => "Alerting Service";

    /// <inheritdoc/>
    public override string ComponentType => "AlertingService";

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
        var indexKeysDefinition = Builders<AlertRecord>.IndexKeys
            .Ascending(x => x.Timestamp)
            .Ascending(x => x.Name)
            .Ascending(x => x.Severity);

        await _alertCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<AlertRecord>(indexKeysDefinition));

        Logger.Info("Alerting service initialized");
    }

    /// <summary>
    /// Registers an alert rule.
    /// </summary>
    /// <param name="name">The alert rule name.</param>
    /// <param name="condition">The alert condition function.</param>
    /// <param name="severity">The alert severity.</param>
    /// <param name="description">The alert description.</param>
    /// <param name="tags">The alert tags.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RegisterAlertRuleAsync(
        string name,
        Func<Task<bool>> condition,
        AlertSeverity severity,
        string description = "",
        IEnumerable<string>? tags = null)
    {
        using var span = TelemetryProvider.CreateSpan("AlertingService.RegisterAlertRule");
        span.SetAttribute("alert.rule.name", name);
        span.SetAttribute("alert.severity", severity.ToString());

        try
        {
            Logger.Info($"Registering alert rule: {name}");
            _alertManager.RegisterAlertRule(name, condition, severity, description, tags);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to register alert rule: {name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Evaluates all alert rules.
    /// </summary>
    /// <returns>The active alerts.</returns>
    public async Task<IReadOnlyDictionary<string, Alert>> EvaluateAlertRulesAsync()
    {
        using var span = TelemetryProvider.CreateSpan("AlertingService.EvaluateAlertRules");

        try
        {
            Logger.Info("Evaluating alert rules");
            var activeAlerts = await _alertManager.EvaluateAlertRulesAsync();

            // Record alerts in database
            foreach (var alert in activeAlerts.Values)
            {
                var record = new AlertRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    Name = alert.Name,
                    Description = alert.Description,
                    Severity = alert.Severity,
                    StartTimestamp = alert.StartTimestamp,
                    LastUpdateTimestamp = alert.LastUpdateTimestamp,
                    EndTimestamp = alert.EndTimestamp,
                    Tags = alert.Tags?.ToList()
                };

                await _alertCollection.InsertOneAsync(record);
            }

            Logger.Info($"Alert rules evaluated. Active alerts: {activeAlerts.Count}");
            span.SetStatus(SpanStatus.Ok);
            return activeAlerts;
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to evaluate alert rules", ex);
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

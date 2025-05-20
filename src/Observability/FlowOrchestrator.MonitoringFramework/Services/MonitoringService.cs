using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.ObservabilityBase.Components;
using FlowOrchestrator.ObservabilityBase.Monitoring;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace FlowOrchestrator.MonitoringFramework.Services;

/// <summary>
/// Service for monitoring system health and performance.
/// </summary>
public class MonitoringService : AbstractObservabilityComponent
{
    private readonly IMongoCollection<HealthCheckRecord> _healthCheckCollection;
    private readonly IMongoCollection<ResourceUtilizationRecord> _resourceUtilizationCollection;
    private readonly ConcurrentDictionary<string, HealthStatus> _healthStatuses = new();
    private readonly HealthMonitor _healthMonitor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitoringService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationProvider">The configuration provider.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="mongoClient">The MongoDB client.</param>
    public MonitoringService(
        FlowOrchestrator.Common.Logging.ILogger logger,
        Common.Configuration.IConfigurationProvider configurationProvider,
        Infrastructure.Common.Messaging.IMessageBus messageBus,
        ITelemetryProvider telemetryProvider,
        IMongoClient mongoClient)
        : base(logger, configurationProvider, messageBus, telemetryProvider)
    {
        var database = mongoClient.GetDatabase("FlowOrchestrator");
        _healthCheckCollection = database.GetCollection<HealthCheckRecord>("HealthChecks");
        _resourceUtilizationCollection = database.GetCollection<ResourceUtilizationRecord>("ResourceUtilization");
        _healthMonitor = new HealthMonitor(logger, telemetryProvider);
    }

    /// <inheritdoc/>
    public override string ComponentId => "MonitoringService";

    /// <inheritdoc/>
    public override string ComponentName => "Monitoring Service";

    /// <inheritdoc/>
    public override string ComponentType => "MonitoringService";

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
        var healthCheckIndexKeysDefinition = Builders<HealthCheckRecord>.IndexKeys
            .Ascending(x => x.Timestamp)
            .Ascending(x => x.ComponentId)
            .Ascending(x => x.CheckName);

        await _healthCheckCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<HealthCheckRecord>(healthCheckIndexKeysDefinition));

        var resourceUtilizationIndexKeysDefinition = Builders<ResourceUtilizationRecord>.IndexKeys
            .Ascending(x => x.Timestamp)
            .Ascending(x => x.ComponentId)
            .Ascending(x => x.ResourceType);

        await _resourceUtilizationCollection.Indexes.CreateOneAsync(
            new CreateIndexModel<ResourceUtilizationRecord>(resourceUtilizationIndexKeysDefinition));

        Logger.Info("Monitoring service initialized");
    }

    /// <summary>
    /// Records a health check result.
    /// </summary>
    /// <param name="componentId">The component identifier.</param>
    /// <param name="checkName">The check name.</param>
    /// <param name="status">The status.</param>
    /// <param name="message">The message.</param>
    /// <param name="details">The details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordHealthCheckAsync(
        string componentId,
        string checkName,
        HealthCheckStatus status,
        string message,
        Dictionary<string, object>? details = null)
    {
        using var span = TelemetryProvider.CreateSpan("MonitoringService.RecordHealthCheck");
        span.SetAttribute("component.id", componentId);
        span.SetAttribute("health.check.name", checkName);
        span.SetAttribute("health.check.status", status.ToString());

        try
        {
            // Record in database
            var record = new HealthCheckRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                ComponentId = componentId,
                CheckName = checkName,
                Status = status,
                Message = message,
                Details = details
            };

            await _healthCheckCollection.InsertOneAsync(record);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to record health check {componentId}.{checkName}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }

    /// <summary>
    /// Records resource utilization.
    /// </summary>
    /// <param name="componentId">The component identifier.</param>
    /// <param name="resourceType">The resource type.</param>
    /// <param name="value">The value.</param>
    /// <param name="attributes">The attributes.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordResourceUtilizationAsync(
        string componentId,
        string resourceType,
        double value,
        Dictionary<string, object>? attributes = null)
    {
        using var span = TelemetryProvider.CreateSpan("MonitoringService.RecordResourceUtilization");
        span.SetAttribute("component.id", componentId);
        span.SetAttribute("resource.type", resourceType);
        span.SetAttribute("resource.value", value);

        try
        {
            // Record in database
            var record = new ResourceUtilizationRecord
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                ComponentId = componentId,
                ResourceType = resourceType,
                Value = value,
                Attributes = attributes
            };

            await _resourceUtilizationCollection.InsertOneAsync(record);
            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to record resource utilization {componentId}.{resourceType}", ex);
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

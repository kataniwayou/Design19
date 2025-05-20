using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.MonitoringFramework.Services;
using FlowOrchestrator.ObservabilityBase.Monitoring;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FlowOrchestrator.MonitoringFramework.Controllers;

/// <summary>
/// Controller for monitoring endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MonitoringController : ControllerBase
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly MonitoringService _monitoringService;
    private readonly IMongoCollection<HealthCheckRecord> _healthCheckCollection;
    private readonly IMongoCollection<ResourceUtilizationRecord> _resourceUtilizationCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitoringController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="monitoringService">The monitoring service.</param>
    /// <param name="mongoClient">The MongoDB client.</param>
    public MonitoringController(
        FlowOrchestrator.Common.Logging.ILogger logger,
        ITelemetryProvider telemetryProvider,
        MonitoringService monitoringService,
        IMongoClient mongoClient)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _monitoringService = monitoringService;

        var database = mongoClient.GetDatabase("FlowOrchestrator");
        _healthCheckCollection = database.GetCollection<HealthCheckRecord>("HealthChecks");
        _resourceUtilizationCollection = database.GetCollection<ResourceUtilizationRecord>("ResourceUtilization");
    }

    /// <summary>
    /// Gets the health status.
    /// </summary>
    /// <returns>The health status.</returns>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealthStatus()
    {
        using var span = _telemetryProvider.CreateSpan("MonitoringController.GetHealthStatus");

        try
        {
            _logger.Info("Getting health status");

            var filter = Builders<HealthCheckRecord>.Filter.Empty;
            var sort = Builders<HealthCheckRecord>.Sort.Descending(x => x.Timestamp);
            var healthChecks = await _healthCheckCollection.Find(filter).Sort(sort).Limit(100).ToListAsync();

            var result = healthChecks.GroupBy(x => x.ComponentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.CheckName)
                        .ToDictionary(
                            g2 => g2.Key,
                            g2 => g2.OrderByDescending(x => x.Timestamp).First()));

            span.SetStatus(SpanStatus.Ok);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to get health status", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, "Failed to get health status");
        }
    }

    /// <summary>
    /// Gets the resource utilization.
    /// </summary>
    /// <param name="componentId">The component identifier.</param>
    /// <param name="resourceType">The resource type.</param>
    /// <param name="timeRange">The time range in minutes.</param>
    /// <returns>The resource utilization.</returns>
    [HttpGet("resources")]
    public async Task<IActionResult> GetResourceUtilization(
        [FromQuery] string? componentId = null,
        [FromQuery] string? resourceType = null,
        [FromQuery] int timeRange = 60)
    {
        using var span = _telemetryProvider.CreateSpan("MonitoringController.GetResourceUtilization");
        span.SetAttribute("component.id", componentId ?? "all");
        span.SetAttribute("resource.type", resourceType ?? "all");
        span.SetAttribute("time.range", timeRange);

        try
        {
            _logger.Info($"Getting resource utilization for component: {componentId}, resource: {resourceType}, time range: {timeRange} minutes");

            var startTime = DateTime.UtcNow.AddMinutes(-timeRange);
            var filterBuilder = Builders<ResourceUtilizationRecord>.Filter;
            var filter = filterBuilder.Gte(x => x.Timestamp, startTime);

            if (!string.IsNullOrEmpty(componentId))
            {
                filter = filterBuilder.And(filter, filterBuilder.Eq(x => x.ComponentId, componentId));
            }

            if (!string.IsNullOrEmpty(resourceType))
            {
                filter = filterBuilder.And(filter, filterBuilder.Eq(x => x.ResourceType, resourceType));
            }

            var sort = Builders<ResourceUtilizationRecord>.Sort.Ascending(x => x.Timestamp);
            var resourceUtilization = await _resourceUtilizationCollection.Find(filter).Sort(sort).ToListAsync();

            span.SetStatus(SpanStatus.Ok);
            return Ok(resourceUtilization);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to get resource utilization", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, "Failed to get resource utilization");
        }
    }

    /// <summary>
    /// Records a health check.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The result.</returns>
    [HttpPost("health")]
    public async Task<IActionResult> RecordHealthCheck([FromBody] HealthCheckRequest request)
    {
        using var span = _telemetryProvider.CreateSpan("MonitoringController.RecordHealthCheck");
        span.SetAttribute("component.id", request.ComponentId);
        span.SetAttribute("health.check.name", request.CheckName);
        span.SetAttribute("health.check.status", request.Status.ToString());

        try
        {
            _logger.Info($"Recording health check for component: {request.ComponentId}, check: {request.CheckName}, status: {request.Status}");

            await _monitoringService.RecordHealthCheckAsync(
                request.ComponentId,
                request.CheckName,
                request.Status,
                request.Message,
                request.Details);

            span.SetStatus(SpanStatus.Ok);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to record health check", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, "Failed to record health check");
        }
    }

    /// <summary>
    /// Records resource utilization.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The result.</returns>
    [HttpPost("resources")]
    public async Task<IActionResult> RecordResourceUtilization([FromBody] ResourceUtilizationRequest request)
    {
        using var span = _telemetryProvider.CreateSpan("MonitoringController.RecordResourceUtilization");
        span.SetAttribute("component.id", request.ComponentId);
        span.SetAttribute("resource.type", request.ResourceType);
        span.SetAttribute("resource.value", request.Value);

        try
        {
            _logger.Info($"Recording resource utilization for component: {request.ComponentId}, resource: {request.ResourceType}, value: {request.Value}");

            await _monitoringService.RecordResourceUtilizationAsync(
                request.ComponentId,
                request.ResourceType,
                request.Value,
                request.Attributes);

            span.SetStatus(SpanStatus.Ok);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to record resource utilization", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return StatusCode(500, "Failed to record resource utilization");
        }
    }
}

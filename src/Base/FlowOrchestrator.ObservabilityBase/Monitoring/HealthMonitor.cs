using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;

namespace FlowOrchestrator.ObservabilityBase.Monitoring;

/// <summary>
/// Health monitor.
/// </summary>
public class HealthMonitor
{
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly Dictionary<string, HealthCheck> _healthChecks = new Dictionary<string, HealthCheck>();
    private readonly Dictionary<string, HealthStatus> _healthStatuses = new Dictionary<string, HealthStatus>();

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthMonitor"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    public HealthMonitor(
        ILogger logger,
        ITelemetryProvider telemetryProvider)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
    }

    /// <summary>
    /// Registers a health check.
    /// </summary>
    /// <param name="name">The health check name.</param>
    /// <param name="check">The health check function.</param>
    /// <param name="description">The health check description.</param>
    /// <param name="tags">The health check tags.</param>
    public void RegisterHealthCheck(
        string name,
        Func<Task<HealthCheckResult>> check,
        string description = "",
        IEnumerable<string>? tags = null)
    {
        try
        {
            var healthCheck = new HealthCheck
            {
                Name = name,
                Description = description,
                Check = check,
                Tags = tags?.ToList() ?? new List<string>()
            };

            _healthChecks[name] = healthCheck;
            _healthStatuses[name] = new HealthStatus
            {
                Name = name,
                Status = HealthCheckStatus.Unknown,
                LastCheckTimestamp = DateTime.UtcNow,
                LastStatusChangeTimestamp = DateTime.UtcNow
            };

            _logger.Info($"Registered health check: {name}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to register health check: {name}", ex);
        }
    }

    /// <summary>
    /// Runs all health checks.
    /// </summary>
    /// <returns>The health check results.</returns>
    public async Task<HealthCheckResults> RunHealthChecksAsync()
    {
        using var span = _telemetryProvider.CreateSpan("HealthMonitor.RunHealthChecks");

        try
        {
            _logger.Info("Running health checks");

            var results = new HealthCheckResults
            {
                Timestamp = DateTime.UtcNow,
                Results = new Dictionary<string, HealthCheckResult>()
            };

            foreach (var healthCheck in _healthChecks.Values)
            {
                try
                {
                    var result = await RunHealthCheckAsync(healthCheck);
                    results.Results[healthCheck.Name] = result;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to run health check: {healthCheck.Name}", ex);
                    results.Results[healthCheck.Name] = new HealthCheckResult
                    {
                        Status = HealthCheckStatus.Unhealthy,
                        Description = $"Failed to run health check: {ex.Message}"
                    };
                }
            }

            results.Status = DetermineOverallStatus(results.Results.Values);
            _logger.Info($"Health checks completed. Overall status: {results.Status}");

            span.SetStatus(SpanStatus.Ok);
            return results;
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to run health checks", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);

            return new HealthCheckResults
            {
                Timestamp = DateTime.UtcNow,
                Status = HealthCheckStatus.Unhealthy,
                Results = new Dictionary<string, HealthCheckResult>()
            };
        }
    }

    /// <summary>
    /// Runs a specific health check.
    /// </summary>
    /// <param name="name">The health check name.</param>
    /// <returns>The health check result.</returns>
    public async Task<HealthCheckResult> RunHealthCheckAsync(string name)
    {
        using var span = _telemetryProvider.CreateSpan("HealthMonitor.RunHealthCheck");
        span.SetAttribute("health.check.name", name);

        try
        {
            _logger.Info($"Running health check: {name}");

            if (!_healthChecks.TryGetValue(name, out var healthCheck))
            {
                _logger.Warn($"Health check not found: {name}");
                span.SetStatus(SpanStatus.Error, $"Health check not found: {name}");
                return new HealthCheckResult
                {
                    Status = HealthCheckStatus.Unknown,
                    Description = $"Health check not found: {name}"
                };
            }

            var result = await RunHealthCheckAsync(healthCheck);
            _logger.Info($"Health check completed: {name}. Status: {result.Status}");

            span.SetStatus(SpanStatus.Ok);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to run health check: {name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);

            return new HealthCheckResult
            {
                Status = HealthCheckStatus.Unhealthy,
                Description = $"Failed to run health check: {ex.Message}"
            };
        }
    }

    private async Task<HealthCheckResult> RunHealthCheckAsync(HealthCheck healthCheck)
    {
        using var span = _telemetryProvider.CreateSpan("HealthMonitor.RunHealthCheck");
        span.SetAttribute("health.check.name", healthCheck.Name);

        try
        {
            var startTime = DateTime.UtcNow;
            var result = await healthCheck.Check();
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Update health status
            if (_healthStatuses.TryGetValue(healthCheck.Name, out var healthStatus))
            {
                var previousStatus = healthStatus.Status;
                healthStatus.Status = result.Status;
                healthStatus.LastCheckTimestamp = endTime;
                healthStatus.LastCheckDuration = duration;

                if (previousStatus != result.Status)
                {
                    healthStatus.LastStatusChangeTimestamp = endTime;
                }
            }

            // Record telemetry
            _telemetryProvider.RecordMetric(
                "health.check.duration",
                duration.TotalMilliseconds,
                new Dictionary<string, object>
                {
                    { "health.check.name", healthCheck.Name },
                    { "health.check.status", result.Status.ToString() }
                });

            _telemetryProvider.RecordMetric(
                "health.check.status",
                (int)result.Status,
                new Dictionary<string, object>
                {
                    { "health.check.name", healthCheck.Name }
                });

            span.SetStatus(SpanStatus.Ok);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to run health check: {healthCheck.Name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);

            // Update health status
            if (_healthStatuses.TryGetValue(healthCheck.Name, out var healthStatus))
            {
                var previousStatus = healthStatus.Status;
                healthStatus.Status = HealthCheckStatus.Unhealthy;
                healthStatus.LastCheckTimestamp = DateTime.UtcNow;

                if (previousStatus != HealthCheckStatus.Unhealthy)
                {
                    healthStatus.LastStatusChangeTimestamp = DateTime.UtcNow;
                }
            }

            return new HealthCheckResult
            {
                Status = HealthCheckStatus.Unhealthy,
                Description = $"Failed to run health check: {ex.Message}"
            };
        }
    }

    private HealthCheckStatus DetermineOverallStatus(IEnumerable<HealthCheckResult> results)
    {
        if (!results.Any())
        {
            return HealthCheckStatus.Unknown;
        }

        if (results.Any(r => r.Status == HealthCheckStatus.Unhealthy))
        {
            return HealthCheckStatus.Unhealthy;
        }

        if (results.Any(r => r.Status == HealthCheckStatus.Degraded))
        {
            return HealthCheckStatus.Degraded;
        }

        if (results.All(r => r.Status == HealthCheckStatus.Healthy))
        {
            return HealthCheckStatus.Healthy;
        }

        return HealthCheckStatus.Degraded;
    }

    /// <summary>
    /// Gets all health statuses.
    /// </summary>
    /// <returns>The health statuses.</returns>
    public IReadOnlyDictionary<string, HealthStatus> GetHealthStatuses()
    {
        return _healthStatuses;
    }

    /// <summary>
    /// Gets a health status.
    /// </summary>
    /// <param name="name">The health check name.</param>
    /// <returns>The health status, or null if not found.</returns>
    public HealthStatus? GetHealthStatus(string name)
    {
        return _healthStatuses.TryGetValue(name, out var healthStatus) ? healthStatus : null;
    }
}

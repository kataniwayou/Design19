using FlowOrchestrator.AlertingSystem.Services;
using FlowOrchestrator.Common.Logging;

namespace FlowOrchestrator.AlertingSystem;

/// <summary>
/// Worker service for the alerting system.
/// </summary>
public class Worker : BackgroundService
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly AlertingService _alertingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="alertingService">The alerting service.</param>
    public Worker(
        FlowOrchestrator.Common.Logging.ILogger logger,
        AlertingService alertingService)
    {
        _logger = logger;
        _alertingService = alertingService;
    }

    /// <summary>
    /// Executes the worker.
    /// </summary>
    /// <param name="stoppingToken">The stopping token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.Info("Initializing alerting service");
            await _alertingService.InitializeAsync();
            _logger.Info("Alerting service initialized");

            // Register default alert rules
            await RegisterDefaultAlertRulesAsync();

            // Periodically evaluate alert rules
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.Info("Evaluating alert rules");
                    var activeAlerts = await _alertingService.EvaluateAlertRulesAsync();
                    _logger.Info($"Active alerts: {activeAlerts.Count}");

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in alerting service worker", ex);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Fatal error in alerting service worker", ex);
            throw;
        }
    }

    /// <summary>
    /// Registers the default alert rules.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task RegisterDefaultAlertRulesAsync()
    {
        _logger.Info("Registering default alert rules");

        // Example: Register a rule for high CPU usage
        await _alertingService.RegisterAlertRuleAsync(
            "HighCpuUsage",
            async () =>
            {
                // This is a placeholder for the actual condition
                // In a real implementation, this would check metrics or other data sources
                return false;
            },
            ObservabilityBase.Monitoring.AlertSeverity.Warning,
            "CPU usage is above 80%",
            new[] { "performance", "resource" });

        // Example: Register a rule for service health
        await _alertingService.RegisterAlertRuleAsync(
            "ServiceUnhealthy",
            async () =>
            {
                // This is a placeholder for the actual condition
                return false;
            },
            ObservabilityBase.Monitoring.AlertSeverity.Critical,
            "One or more services are unhealthy",
            new[] { "health", "service" });

        _logger.Info("Default alert rules registered");
    }
}

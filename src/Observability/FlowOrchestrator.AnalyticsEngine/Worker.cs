using FlowOrchestrator.AnalyticsEngine.Services;
using FlowOrchestrator.Common.Logging;

namespace FlowOrchestrator.AnalyticsEngine;

/// <summary>
/// Worker service for the analytics engine.
/// </summary>
public class Worker : BackgroundService
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly AnalyticsService _analyticsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="analyticsService">The analytics service.</param>
    public Worker(
        FlowOrchestrator.Common.Logging.ILogger logger,
        AnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
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
            _logger.Info("Initializing analytics service");
            await _analyticsService.InitializeAsync();
            _logger.Info("Analytics service initialized");

            // Periodically generate reports
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.Info("Generating scheduled reports");
                    await GenerateScheduledReportsAsync();
                    _logger.Info("Scheduled reports generated");

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in analytics service worker", ex);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Fatal error in analytics service worker", ex);
            throw;
        }
    }

    /// <summary>
    /// Generates scheduled reports.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task GenerateScheduledReportsAsync()
    {
        // Generate daily flow execution report
        await _analyticsService.GenerateReportAsync(
            "DailyFlowExecutionReport",
            new Dictionary<string, object>
            {
                { "date", DateTime.UtcNow.Date }
            });

        // Generate system performance report
        await _analyticsService.GenerateReportAsync(
            "SystemPerformanceReport",
            new Dictionary<string, object>
            {
                { "timeRange", 24 } // Hours
            });

        // Generate error summary report
        await _analyticsService.GenerateReportAsync(
            "ErrorSummaryReport",
            new Dictionary<string, object>
            {
                { "timeRange", 24 } // Hours
            });
    }
}

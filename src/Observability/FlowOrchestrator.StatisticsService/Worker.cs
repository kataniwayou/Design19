using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.StatisticsService.Services;

namespace FlowOrchestrator.StatisticsService;

/// <summary>
/// Worker service for the statistics service.
/// </summary>
public class Worker : BackgroundService
{
    private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
    private readonly Services.StatisticsService _statisticsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="statisticsService">The statistics service.</param>
    public Worker(
        FlowOrchestrator.Common.Logging.ILogger logger,
        Services.StatisticsService statisticsService)
    {
        _logger = logger;
        _statisticsService = statisticsService;
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
            _logger.Info("Initializing statistics service");
            await _statisticsService.InitializeAsync();
            _logger.Info("Statistics service initialized");

            // Periodically clean up old statistics records
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.Info("Statistics service running");

                    // Add any periodic tasks here

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in statistics service worker", ex);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Fatal error in statistics service worker", ex);
            throw;
        }
    }
}
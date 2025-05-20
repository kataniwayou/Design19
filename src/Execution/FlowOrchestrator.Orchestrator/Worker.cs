using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.Observability;
using FlowOrchestrator.Orchestrator.Domain.Entities;
using FlowOrchestrator.Orchestrator.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlowOrchestrator.Orchestrator
{
    /// <summary>
    /// Worker service for the orchestrator.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IFlowExecutionService _executionService;
        private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
        private readonly ITelemetryProvider _telemetryProvider;
        private Timer? _retryTimer;
        private readonly TimeSpan _retryInterval = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        /// <param name="executionService">The execution service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="telemetryProvider">The telemetry provider.</param>
        public Worker(
            IFlowExecutionService executionService,
            FlowOrchestrator.Common.Logging.ILogger logger,
            ITelemetryProvider telemetryProvider)
        {
            _executionService = executionService ?? throw new ArgumentNullException(nameof(executionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
        }

        /// <summary>
        /// Executes the worker.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Orchestrator worker starting");

            try
            {
                // Start the retry timer
                _retryTimer = new Timer(ProcessRetries, null, TimeSpan.Zero, _retryInterval);

                // Wait for cancellation
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Error("Error starting Orchestrator worker", ex);
                throw;
            }
        }

        /// <summary>
        /// Called when the worker is stopping.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Orchestrator worker stopping");
            _retryTimer?.Change(Timeout.Infinite, 0);
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Processes flow executions scheduled for retry.
        /// </summary>
        /// <param name="state">The state.</param>
        private async void ProcessRetries(object? state)
        {
            using var span = _telemetryProvider.CreateSpan("Worker.ProcessRetries");
            // Use the timer helper for measuring operation duration
            using var timer = _telemetryProvider.Timer("orchestrator.retry.duration");

            try
            {
                // Get flow executions scheduled for retry
                var executions = await _executionService.GetExecutionsScheduledForRetryAsync();
                var count = 0;

                // Record the number of executions scheduled for retry
                var scheduledCount = executions.Count();
                _telemetryProvider.RecordMetric("orchestrator.retry.scheduled.gauge", (double)scheduledCount, new Dictionary<string, object>
                {
                    { "source", "worker" }
                });

                foreach (var execution in executions)
                {
                    try
                    {
                        // Start the execution
                        await _executionService.StartExecutionAsync(execution.Id);
                        count++;

                        // Record successful retry
                        _telemetryProvider.RecordCounter("orchestrator.retry.success", 1, new Dictionary<string, object>
                        {
                            { "flow.id", execution.FlowId },
                            { "execution.id", execution.Id }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error retrying flow execution {execution.Id}: {ex.Message}", ex);

                        // Record failed retry
                        _telemetryProvider.RecordCounter("orchestrator.retry.failure", 1, new Dictionary<string, object>
                        {
                            { "flow.id", execution.FlowId },
                            { "execution.id", execution.Id },
                            { "error.type", ex.GetType().Name }
                        });
                    }
                }

                if (count > 0)
                {
                    _logger.Info($"Retried {count} flow executions");

                    // Record total retried executions
                    _telemetryProvider.RecordCounter("orchestrator.retry.total", count);
                }

                span.SetStatus(SpanStatus.Ok);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing retries: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);

                // Record processing error
                _telemetryProvider.RecordCounter("orchestrator.retry.processing.error", 1, new Dictionary<string, object>
                {
                    { "error.type", ex.GetType().Name }
                });
            }
        }
    }
}

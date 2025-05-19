using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.FlowExecutionEngine.Models;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlowOrchestrator.FlowExecutionEngine.Executors
{
    /// <summary>
    /// Implementation of the exporter executor.
    /// </summary>
    public class ExporterExecutor : IExporterExecutor
    {
        private readonly IMongoRepository<DestinationAssignmentEntity> _destinationAssignmentRepository;
        private readonly IExporterFactory _exporterFactory;
        private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
        private readonly ITelemetryProvider _telemetryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExporterExecutor"/> class.
        /// </summary>
        /// <param name="destinationAssignmentRepository">The destination assignment repository.</param>
        /// <param name="exporterFactory">The exporter factory.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="telemetryProvider">The telemetry provider.</param>
        public ExporterExecutor(
            IMongoRepository<DestinationAssignmentEntity> destinationAssignmentRepository,
            IExporterFactory exporterFactory,
            FlowOrchestrator.Common.Logging.ILogger logger,
            ITelemetryProvider telemetryProvider)
        {
            _destinationAssignmentRepository = destinationAssignmentRepository ?? throw new ArgumentNullException(nameof(destinationAssignmentRepository));
            _exporterFactory = exporterFactory ?? throw new ArgumentNullException(nameof(exporterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
        }

        /// <inheritdoc/>
        public async Task<ExportResult> ExecuteExporterAsync(FlowExecutionContext context, string destinationAssignmentId)
        {
            var executionId = context.ExecutionId;

            using var span = _telemetryProvider.CreateSpan("ExporterExecutor.ExecuteExporter");
            span.SetAttribute("flow.execution.id", executionId);
            span.SetAttribute("destination.assignment.id", destinationAssignmentId);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Info($"Executing exporter for flow execution {executionId} with destination assignment {destinationAssignmentId}");

                // Get the destination assignment
                var destinationAssignment = await _destinationAssignmentRepository.GetByIdAsync(destinationAssignmentId);
                if (destinationAssignment == null)
                {
                    throw new InvalidOperationException($"Destination assignment {destinationAssignmentId} not found");
                }

                // Create the exporter
                var exporter = _exporterFactory.CreateExporter(destinationAssignment.ExporterType);
                if (exporter == null)
                {
                    throw new InvalidOperationException($"Exporter of type {destinationAssignment.ExporterType} could not be created");
                }

                // Configure the exporter
                var configJson = System.Text.Json.JsonSerializer.Serialize(destinationAssignment.Configuration);
                exporter.Configure(configJson);

                // Get the processed data
                if (!context.TryGetData<object>("ProcessedData", out var processedData))
                {
                    throw new InvalidOperationException("Processed data not found in context");
                }

                // Execute the export operation
                await exporter.ExportAsync(processedData);
                var recordCount = exporter.GetRecordCount(processedData);

                // Store the export result in the context
                var exportResultKey = $"ExportResult_{destinationAssignmentId}";
                context.SetMetadata(exportResultKey, "Success");
                context.SetMetadata($"{exportResultKey}_RecordCount", recordCount.ToString());
                context.SetMetadata($"{exportResultKey}_ExporterType", destinationAssignment.ExporterType);
                context.SetMetadata($"{exportResultKey}_ExporterName", destinationAssignment.Name);

                stopwatch.Stop();
                var executionTimeMs = stopwatch.ElapsedMilliseconds;
                context.SetMetadata($"{exportResultKey}_ExecutionTimeMs", executionTimeMs.ToString());

                _logger.Info($"Successfully executed exporter for flow execution {executionId} with destination assignment {destinationAssignmentId}. Exported {recordCount} records in {executionTimeMs}ms");
                span.SetStatus(SpanStatus.Ok);

                return ExportResult.CreateSuccess(destinationAssignmentId, "ProcessedData", recordCount, executionTimeMs);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var executionTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.Error($"Error executing exporter for flow execution {executionId} with destination assignment {destinationAssignmentId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);

                // Store the export error in the context
                var exportResultKey = $"ExportResult_{destinationAssignmentId}";
                context.SetMetadata(exportResultKey, "Error");
                context.SetMetadata($"{exportResultKey}_ErrorMessage", ex.Message);
                context.SetMetadata($"{exportResultKey}_ExecutionTimeMs", executionTimeMs.ToString());

                return ExportResult.CreateFailure(destinationAssignmentId, ex.Message, executionTimeMs);
            }
        }

        /// <inheritdoc/>
        public async Task<List<ExportResult>> ExecuteExportersAsync(FlowExecutionContext context)
        {
            var executionId = context.ExecutionId;
            var destinationAssignmentIds = context.DestinationAssignmentIds;

            using var span = _telemetryProvider.CreateSpan("ExporterExecutor.ExecuteExporters");
            span.SetAttribute("flow.execution.id", executionId);
            span.SetAttribute("destination.assignment.count", destinationAssignmentIds.Count);

            try
            {
                _logger.Info($"Executing exporters for flow execution {executionId} with {destinationAssignmentIds.Count} destination assignments");

                var tasks = new List<Task<ExportResult>>();
                foreach (var destinationAssignmentId in destinationAssignmentIds)
                {
                    tasks.Add(ExecuteExporterAsync(context, destinationAssignmentId));
                }

                var results = await Task.WhenAll(tasks);
                var successCount = results.Count(r => r.Success);
                var failureCount = results.Count(r => !r.Success);

                _logger.Info($"Completed executing exporters for flow execution {executionId}. Success: {successCount}, Failure: {failureCount}");
                span.SetStatus(SpanStatus.Ok);

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error executing exporters for flow execution {executionId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }
    }
}

using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.FlowExecutionEngine.Models;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FlowOrchestrator.FlowExecutionEngine.Executors
{
    /// <summary>
    /// Implementation of the importer executor.
    /// </summary>
    public class ImporterExecutor : IImporterExecutor
    {
        private readonly IMongoRepository<SourceAssignmentEntity> _sourceAssignmentRepository;
        private readonly IImporterFactory _importerFactory;
        private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
        private readonly ITelemetryProvider _telemetryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImporterExecutor"/> class.
        /// </summary>
        /// <param name="sourceAssignmentRepository">The source assignment repository.</param>
        /// <param name="importerFactory">The importer factory.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="telemetryProvider">The telemetry provider.</param>
        public ImporterExecutor(
            IMongoRepository<SourceAssignmentEntity> sourceAssignmentRepository,
            IImporterFactory importerFactory,
            FlowOrchestrator.Common.Logging.ILogger logger,
            ITelemetryProvider telemetryProvider)
        {
            _sourceAssignmentRepository = sourceAssignmentRepository ?? throw new ArgumentNullException(nameof(sourceAssignmentRepository));
            _importerFactory = importerFactory ?? throw new ArgumentNullException(nameof(importerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
        }

        /// <inheritdoc/>
        public async Task<ImportResult> ExecuteImporterAsync(FlowExecutionContext context)
        {
            var executionId = context.ExecutionId;
            var sourceAssignmentId = context.SourceAssignmentId;

            using var span = _telemetryProvider.CreateSpan("ImporterExecutor.ExecuteImporter");
            span.SetAttribute("flow.execution.id", executionId);
            span.SetAttribute("source.assignment.id", sourceAssignmentId);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Info($"Executing importer for flow execution {executionId} with source assignment {sourceAssignmentId}");

                // Get the source assignment
                var sourceAssignment = await _sourceAssignmentRepository.GetByIdAsync(sourceAssignmentId);
                if (sourceAssignment == null)
                {
                    throw new InvalidOperationException($"Source assignment {sourceAssignmentId} not found");
                }

                // Create the importer
                var importer = _importerFactory.CreateImporter(sourceAssignment.ImporterType);
                if (importer == null)
                {
                    throw new InvalidOperationException($"Importer of type {sourceAssignment.ImporterType} could not be created");
                }

                // Configure the importer
                var configJson = System.Text.Json.JsonSerializer.Serialize(sourceAssignment.Configuration);
                importer.Configure(configJson);

                // Execute the import operation
                var importedData = await importer.ImportAsync();
                var recordCount = importer.GetRecordCount(importedData);

                // Store the imported data in the context
                context.SetData("ImportedData", importedData);
                context.SetMetadata("ImportedRecordCount", recordCount.ToString());
                context.SetMetadata("ImporterType", sourceAssignment.ImporterType);
                context.SetMetadata("ImporterName", sourceAssignment.Name);

                stopwatch.Stop();
                var executionTimeMs = stopwatch.ElapsedMilliseconds;
                context.SetMetadata("ImportExecutionTimeMs", executionTimeMs.ToString());

                _logger.Info($"Successfully executed importer for flow execution {executionId} with source assignment {sourceAssignmentId}. Imported {recordCount} records in {executionTimeMs}ms");
                span.SetStatus(SpanStatus.Ok);

                return ImportResult.CreateSuccess("ImportedData", recordCount, executionTimeMs);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var executionTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.Error($"Error executing importer for flow execution {executionId} with source assignment {sourceAssignmentId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);

                return ImportResult.CreateFailure(ex.Message, executionTimeMs);
            }
        }
    }
}

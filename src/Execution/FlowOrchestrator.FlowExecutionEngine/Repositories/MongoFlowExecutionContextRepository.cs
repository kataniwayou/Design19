using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.FlowExecutionEngine.Models;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FlowOrchestrator.FlowExecutionEngine.Repositories
{
    /// <summary>
    /// MongoDB implementation of the flow execution context repository.
    /// </summary>
    public class MongoFlowExecutionContextRepository : IFlowExecutionContextRepository
    {
        private readonly IMongoRepository<FlowExecutionContext> _repository;
        private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
        private readonly ITelemetryProvider _telemetryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoFlowExecutionContextRepository"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="telemetryProvider">The telemetry provider.</param>
        public MongoFlowExecutionContextRepository(
            IMongoRepository<FlowExecutionContext> repository,
            FlowOrchestrator.Common.Logging.ILogger logger,
            ITelemetryProvider telemetryProvider)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
        }

        /// <inheritdoc/>
        public async Task<FlowExecutionContext?> GetByExecutionIdAsync(string executionId)
        {
            using var span = _telemetryProvider.CreateSpan("MongoFlowExecutionContextRepository.GetByExecutionId");
            span.SetAttribute("flow.execution.id", executionId);

            try
            {
                var result = await _repository.GetByFilterAsync(c => c.ExecutionId == executionId);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting flow execution context by execution id {executionId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FlowExecutionContext?> GetByIdAsync(string id)
        {
            using var span = _telemetryProvider.CreateSpan("MongoFlowExecutionContextRepository.GetById");
            span.SetAttribute("flow.execution.context.id", id);

            try
            {
                var result = await _repository.GetByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting flow execution context by id {id}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FlowExecutionContext> CreateAsync(FlowExecutionContext context)
        {
            using var span = _telemetryProvider.CreateSpan("MongoFlowExecutionContextRepository.Create");
            span.SetAttribute("flow.execution.id", context.ExecutionId);

            try
            {
                context.UpdatedAt = DateTime.UtcNow;
                var result = await _repository.CreateAsync(context);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating flow execution context: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FlowExecutionContext> UpdateAsync(FlowExecutionContext context)
        {
            using var span = _telemetryProvider.CreateSpan("MongoFlowExecutionContextRepository.Update");
            span.SetAttribute("flow.execution.id", context.ExecutionId);

            try
            {
                context.UpdatedAt = DateTime.UtcNow;
                var result = await _repository.UpdateAsync(context);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating flow execution context {context.ExecutionId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string executionId)
        {
            using var span = _telemetryProvider.CreateSpan("MongoFlowExecutionContextRepository.Delete");
            span.SetAttribute("flow.execution.id", executionId);

            try
            {
                await _repository.DeleteAsync(executionId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting flow execution context {executionId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }
    }
}

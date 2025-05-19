using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Domain.Entities;
using FlowOrchestrator.Infrastructure.Common.MongoDB;
using FlowOrchestrator.Infrastructure.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowOrchestrator.FlowExecutionEngine.Executors
{
    /// <summary>
    /// Implementation of the processor factory.
    /// </summary>
    public class ProcessorFactory : IProcessorFactory
    {
        private readonly IMongoRepository<ProcessorEntity> _processorRepository;
        private readonly FlowOrchestrator.Common.Logging.ILogger _logger;
        private readonly ITelemetryProvider _telemetryProvider;
        private readonly Dictionary<string, Type> _processorTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorFactory"/> class.
        /// </summary>
        /// <param name="processorRepository">The processor repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="telemetryProvider">The telemetry provider.</param>
        public ProcessorFactory(
            IMongoRepository<ProcessorEntity> processorRepository,
            FlowOrchestrator.Common.Logging.ILogger logger,
            ITelemetryProvider telemetryProvider)
        {
            _processorRepository = processorRepository ?? throw new ArgumentNullException(nameof(processorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
            _processorTypes = new Dictionary<string, Type>
            {
                { "FilterProcessor", typeof(FilterProcessor) },
                { "TransformProcessor", typeof(TransformProcessor) },
                { "AggregateProcessor", typeof(AggregateProcessor) }
            };
        }

        /// <inheritdoc/>
        public IProcessor CreateProcessor(string processorId)
        {
            using var span = _telemetryProvider.CreateSpan("ProcessorFactory.CreateProcessor");
            span.SetAttribute("processor.id", processorId);

            try
            {
                // Get the processor entity
                var processorEntity = _processorRepository.GetByIdAsync(processorId).Result;
                if (processorEntity == null)
                {
                    throw new InvalidOperationException($"Processor {processorId} not found");
                }

                var processorType = processorEntity.ProcessorType;
                if (!_processorTypes.TryGetValue(processorType, out var type))
                {
                    throw new InvalidOperationException($"Processor type {processorType} not found");
                }

                var processor = Activator.CreateInstance(type) as IProcessor;
                if (processor == null)
                {
                    throw new InvalidOperationException($"Failed to create processor of type {processorType}");
                }

                return processor;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating processor {processorId}: {ex.Message}", ex);
                span.SetStatus(SpanStatus.Error, ex.Message);
                span.RecordException(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public int GetRecordCount(object data)
        {
            if (data is List<Dictionary<string, object>> list)
            {
                return list.Count;
            }

            return 0;
        }
    }

    /// <summary>
    /// Filter processor implementation.
    /// </summary>
    public class FilterProcessor : IProcessor
    {
        /// <inheritdoc/>
        public Task<object> ProcessAsync(object data)
        {
            if (data is List<Dictionary<string, object>> list)
            {
                // Filter items with value > 200
                var filteredList = list.FindAll(item => Convert.ToInt32(item["Value"]) > 200);
                return Task.FromResult<object>(filteredList);
            }

            return Task.FromResult(data);
        }
    }

    /// <summary>
    /// Transform processor implementation.
    /// </summary>
    public class TransformProcessor : IProcessor
    {
        /// <inheritdoc/>
        public Task<object> ProcessAsync(object data)
        {
            if (data is List<Dictionary<string, object>> list)
            {
                // Transform items by doubling the value
                foreach (var item in list)
                {
                    item["Value"] = Convert.ToInt32(item["Value"]) * 2;
                }
                return Task.FromResult<object>(list);
            }

            return Task.FromResult(data);
        }
    }

    /// <summary>
    /// Aggregate processor implementation.
    /// </summary>
    public class AggregateProcessor : IProcessor
    {
        /// <inheritdoc/>
        public Task<object> ProcessAsync(object data)
        {
            if (data is List<Dictionary<string, object>> list)
            {
                // Aggregate items by summing the values
                var sum = 0;
                foreach (var item in list)
                {
                    sum += Convert.ToInt32(item["Value"]);
                }

                var result = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "Id", 1 }, { "Name", "Aggregated Result" }, { "Value", sum } }
                };

                return Task.FromResult<object>(result);
            }

            return Task.FromResult(data);
        }
    }
}

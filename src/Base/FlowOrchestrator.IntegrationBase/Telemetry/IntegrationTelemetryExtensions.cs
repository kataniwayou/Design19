using FlowOrchestrator.Infrastructure.Common.Telemetry;
using FlowOrchestrator.IntegrationBase.Messaging;

namespace FlowOrchestrator.IntegrationBase.Telemetry;

/// <summary>
/// Extension methods for integration telemetry.
/// </summary>
public static class IntegrationTelemetryExtensions
{
    /// <summary>
    /// Records import statistics.
    /// </summary>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="componentId">The component identifier.</param>
    /// <param name="componentName">The component name.</param>
    /// <param name="statistics">The import statistics.</param>
    public static void RecordImportStatistics(
        this ITelemetryProvider telemetryProvider,
        string componentId,
        string componentName,
        ImportStatistics statistics)
    {
        telemetryProvider.RecordMetric("import.duration", statistics.Duration.TotalMilliseconds, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Importer" }
        });

        telemetryProvider.RecordMetric("import.records", statistics.RecordsImported, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Importer" }
        });

        telemetryProvider.RecordMetric("import.bytes", statistics.BytesImported, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Importer" }
        });

        telemetryProvider.RecordMetric("import.errors", statistics.ErrorCount, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Importer" }
        });

        telemetryProvider.RecordMetric("import.warnings", statistics.WarningCount, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Importer" }
        });
    }

    /// <summary>
    /// Records export statistics.
    /// </summary>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="componentId">The component identifier.</param>
    /// <param name="componentName">The component name.</param>
    /// <param name="statistics">The export statistics.</param>
    public static void RecordExportStatistics(
        this ITelemetryProvider telemetryProvider,
        string componentId,
        string componentName,
        ExportStatistics statistics)
    {
        telemetryProvider.RecordMetric("export.duration", statistics.Duration.TotalMilliseconds, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Exporter" }
        });

        telemetryProvider.RecordMetric("export.records", statistics.RecordsExported, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Exporter" }
        });

        telemetryProvider.RecordMetric("export.bytes", statistics.BytesExported, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Exporter" }
        });

        telemetryProvider.RecordMetric("export.errors", statistics.ErrorCount, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Exporter" }
        });

        telemetryProvider.RecordMetric("export.warnings", statistics.WarningCount, new Dictionary<string, object>
        {
            { "component.id", componentId },
            { "component.name", componentName },
            { "component.type", "Exporter" }
        });
    }
}

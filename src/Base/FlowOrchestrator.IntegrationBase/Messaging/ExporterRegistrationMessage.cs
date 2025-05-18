using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents a registration message for an exporter.
/// </summary>
public class ExporterRegistrationMessage : EventBase
{
    /// <summary>
    /// Gets the exporter identifier.
    /// </summary>
    public string ExporterId { get; }

    /// <summary>
    /// Gets the exporter name.
    /// </summary>
    public string ExporterName { get; }

    /// <summary>
    /// Gets the exporter version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the protocol supported by the exporter.
    /// </summary>
    public string Protocol { get; }

    /// <summary>
    /// Gets the data format supported by the exporter.
    /// </summary>
    public string DataFormat { get; }

    /// <summary>
    /// Gets the capabilities of the exporter.
    /// </summary>
    public List<string> Capabilities { get; }

    /// <summary>
    /// Gets the schema definition of the exporter.
    /// </summary>
    public SchemaDefinition Schema { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExporterRegistrationMessage"/> class.
    /// </summary>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <param name="exporterName">The exporter name.</param>
    /// <param name="version">The exporter version.</param>
    /// <param name="protocol">The protocol supported by the exporter.</param>
    /// <param name="dataFormat">The data format supported by the exporter.</param>
    /// <param name="capabilities">The capabilities of the exporter.</param>
    /// <param name="schema">The schema definition of the exporter.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ExporterRegistrationMessage(
        string exporterId,
        string exporterName,
        string version,
        string protocol,
        string dataFormat,
        List<string> capabilities,
        SchemaDefinition schema,
        string correlationId)
        : base(correlationId)
    {
        ExporterId = exporterId;
        ExporterName = exporterName;
        Version = version;
        Protocol = protocol;
        DataFormat = dataFormat;
        Capabilities = capabilities;
        Schema = schema;
    }
}

using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents a registration message for an importer.
/// </summary>
public class ImporterRegistrationMessage : EventBase
{
    /// <summary>
    /// Gets the importer identifier.
    /// </summary>
    public string ImporterId { get; }

    /// <summary>
    /// Gets the importer name.
    /// </summary>
    public string ImporterName { get; }

    /// <summary>
    /// Gets the importer version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the protocol supported by the importer.
    /// </summary>
    public string Protocol { get; }

    /// <summary>
    /// Gets the data format supported by the importer.
    /// </summary>
    public string DataFormat { get; }

    /// <summary>
    /// Gets the capabilities of the importer.
    /// </summary>
    public List<string> Capabilities { get; }

    /// <summary>
    /// Gets the schema definition of the importer.
    /// </summary>
    public SchemaDefinition Schema { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImporterRegistrationMessage"/> class.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <param name="importerName">The importer name.</param>
    /// <param name="version">The importer version.</param>
    /// <param name="protocol">The protocol supported by the importer.</param>
    /// <param name="dataFormat">The data format supported by the importer.</param>
    /// <param name="capabilities">The capabilities of the importer.</param>
    /// <param name="schema">The schema definition of the importer.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ImporterRegistrationMessage(
        string importerId,
        string importerName,
        string version,
        string protocol,
        string dataFormat,
        List<string> capabilities,
        SchemaDefinition schema,
        string correlationId)
        : base(correlationId)
    {
        ImporterId = importerId;
        ImporterName = importerName;
        Version = version;
        Protocol = protocol;
        DataFormat = dataFormat;
        Capabilities = capabilities;
        Schema = schema;
    }
}

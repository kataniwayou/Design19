using FlowOrchestrator.Abstractions.Entities;
using FlowOrchestrator.Abstractions.Interfaces;
using System.Text.Json.Serialization;

namespace FlowOrchestrator.Domain.Entities;

/// <summary>
/// Represents an importer entity in the FlowOrchestrator system.
/// An importer entity represents a service that imports data from external sources.
/// </summary>
public class ImporterEntity : AbstractEntity
{
    /// <summary>
    /// Gets or sets the type of the importer.
    /// </summary>
    public string ImporterType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the protocol used by the importer.
    /// </summary>
    public string Protocol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capabilities of the importer.
    /// </summary>
    public List<string> Capabilities { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the output schema of the importer.
    /// </summary>
    public SchemaDefinition OutputSchema { get; set; } = new SchemaDefinition();

    /// <summary>
    /// Gets or sets the configuration parameters of the importer.
    /// </summary>
    public Dictionary<string, ParameterDefinition> ConfigurationParameters { get; set; } = new Dictionary<string, ParameterDefinition>();

    /// <summary>
    /// Gets or sets the protocol capabilities of the importer.
    /// </summary>
    public ProtocolCapabilities ProtocolCapabilities { get; set; } = new ProtocolCapabilities();

    /// <summary>
    /// Gets or sets the status of the importer.
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the last known address of the importer service.
    /// </summary>
    public string LastKnownAddress { get; set; } = string.Empty;
}

/// <summary>
/// Represents a schema definition in the FlowOrchestrator system.
/// </summary>
public class SchemaDefinition
{
    /// <summary>
    /// Gets or sets the fields of the schema.
    /// </summary>
    public List<SchemaField> Fields { get; set; } = new List<SchemaField>();

    /// <summary>
    /// Gets or sets the schema version.
    /// </summary>
    public string SchemaVersion { get; set; } = "1.0.0";
}

/// <summary>
/// Represents a schema field in the FlowOrchestrator system.
/// </summary>
public class SchemaField
{
    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type of the field.
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the field is required.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the description of the field.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default value of the field.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the validation rules of the field.
    /// </summary>
    public Dictionary<string, object> ValidationRules { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Represents a parameter definition in the FlowOrchestrator system.
/// </summary>
public class ParameterDefinition
{
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type of the parameter.
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the parameter is required.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the description of the parameter.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default value of the parameter.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the validation rules of the parameter.
    /// </summary>
    public Dictionary<string, object> ValidationRules { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Represents protocol capabilities in the FlowOrchestrator system.
/// </summary>
public class ProtocolCapabilities
{
    /// <summary>
    /// Gets or sets a value indicating whether the protocol supports authentication.
    /// </summary>
    public bool SupportsAuthentication { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the protocol supports encryption.
    /// </summary>
    public bool SupportsEncryption { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the protocol supports compression.
    /// </summary>
    public bool SupportsCompression { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the protocol supports batching.
    /// </summary>
    public bool SupportsBatching { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the protocol supports streaming.
    /// </summary>
    public bool SupportsStreaming { get; set; }

    /// <summary>
    /// Gets or sets the supported authentication methods.
    /// </summary>
    public List<string> SupportedAuthenticationMethods { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the supported encryption methods.
    /// </summary>
    public List<string> SupportedEncryptionMethods { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the supported compression methods.
    /// </summary>
    public List<string> SupportedCompressionMethods { get; set; } = new List<string>();
}

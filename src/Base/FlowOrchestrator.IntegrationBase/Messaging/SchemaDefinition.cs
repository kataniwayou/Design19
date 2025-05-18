namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents a schema definition.
/// </summary>
public class SchemaDefinition
{
    /// <summary>
    /// Gets or sets the schema name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schema version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schema format.
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schema definition.
    /// </summary>
    public string Definition { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schema fields.
    /// </summary>
    public List<SchemaField> Fields { get; set; } = new List<SchemaField>();
}

/// <summary>
/// Represents a schema field.
/// </summary>
public class SchemaField
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the field is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the field description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field default value.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the field constraints.
    /// </summary>
    public Dictionary<string, object> Constraints { get; set; } = new Dictionary<string, object>();
}

namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents the result of an import operation.
/// </summary>
public class ImportResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the import operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the imported data.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the data format.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data schema.
    /// </summary>
    public SchemaDefinition? Schema { get; set; }

    /// <summary>
    /// Gets or sets the import statistics.
    /// </summary>
    public ImportStatistics Statistics { get; set; } = new ImportStatistics();
}

/// <summary>
/// Represents statistics for an import operation.
/// </summary>
public class ImportStatistics
{
    /// <summary>
    /// Gets or sets the start time of the import operation.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the end time of the import operation.
    /// </summary>
    public DateTime EndTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the number of records imported.
    /// </summary>
    public int RecordsImported { get; set; }

    /// <summary>
    /// Gets or sets the number of bytes imported.
    /// </summary>
    public long BytesImported { get; set; }

    /// <summary>
    /// Gets or sets the number of errors encountered during the import operation.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the number of warnings encountered during the import operation.
    /// </summary>
    public int WarningCount { get; set; }

    /// <summary>
    /// Gets the duration of the import operation.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;
}

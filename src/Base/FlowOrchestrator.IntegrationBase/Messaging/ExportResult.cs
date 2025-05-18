namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents the result of an export operation.
/// </summary>
public class ExportResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the export operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the destination identifier.
    /// </summary>
    public string DestinationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination reference.
    /// </summary>
    public string? DestinationReference { get; set; }

    /// <summary>
    /// Gets or sets the export statistics.
    /// </summary>
    public ExportStatistics Statistics { get; set; } = new ExportStatistics();
}

/// <summary>
/// Represents statistics for an export operation.
/// </summary>
public class ExportStatistics
{
    /// <summary>
    /// Gets or sets the start time of the export operation.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the end time of the export operation.
    /// </summary>
    public DateTime EndTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the number of records exported.
    /// </summary>
    public int RecordsExported { get; set; }

    /// <summary>
    /// Gets or sets the number of bytes exported.
    /// </summary>
    public long BytesExported { get; set; }

    /// <summary>
    /// Gets or sets the number of errors encountered during the export operation.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the number of warnings encountered during the export operation.
    /// </summary>
    public int WarningCount { get; set; }

    /// <summary>
    /// Gets the duration of the export operation.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;
}

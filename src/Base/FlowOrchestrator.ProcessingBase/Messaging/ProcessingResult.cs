namespace FlowOrchestrator.ProcessingBase.Messaging;

/// <summary>
/// Represents the result of a process operation.
/// </summary>
public class ProcessingResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the process operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the processed data.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the data format.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the processing statistics.
    /// </summary>
    public ProcessingStatistics Statistics { get; set; } = new ProcessingStatistics();

    /// <summary>
    /// Gets or sets the validation results.
    /// </summary>
    public ValidationResults ValidationResults { get; set; } = new ValidationResults();

    /// <summary>
    /// Gets or sets the transformation metadata.
    /// </summary>
    public Dictionary<string, object> TransformationMetadata { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Represents statistics for a process operation.
/// </summary>
public class ProcessingStatistics
{
    /// <summary>
    /// Gets or sets the start time of the process operation.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the end time of the process operation.
    /// </summary>
    public DateTime EndTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the number of records processed.
    /// </summary>
    public int RecordsProcessed { get; set; }

    /// <summary>
    /// Gets or sets the number of bytes processed.
    /// </summary>
    public long BytesProcessed { get; set; }

    /// <summary>
    /// Gets or sets the number of errors encountered during the process operation.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the number of warnings encountered during the process operation.
    /// </summary>
    public int WarningCount { get; set; }

    /// <summary>
    /// Gets the duration of the process operation.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;
}

/// <summary>
/// Represents validation results for a process operation.
/// </summary>
public class ValidationResults
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Gets or sets the validation errors.
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

    /// <summary>
    /// Gets or sets the validation warnings.
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new List<ValidationWarning>();
}

/// <summary>
/// Represents a validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field path.
    /// </summary>
    public string? FieldPath { get; set; }
}

/// <summary>
/// Represents a validation warning.
/// </summary>
public class ValidationWarning
{
    /// <summary>
    /// Gets or sets the warning code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the warning message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field path.
    /// </summary>
    public string? FieldPath { get; set; }
}

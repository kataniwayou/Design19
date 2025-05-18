namespace FlowOrchestrator.Common.ErrorHandling;

/// <summary>
/// Base exception class for all FlowOrchestrator exceptions.
/// </summary>
public class FlowOrchestratorException : Exception
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the severity of the error.
    /// </summary>
    public ErrorSeverity Severity { get; }

    /// <summary>
    /// Gets the recovery strategy.
    /// </summary>
    public RecoveryStrategy RecoveryStrategy { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowOrchestratorException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="severity">The severity of the error.</param>
    /// <param name="recoveryStrategy">The recovery strategy.</param>
    public FlowOrchestratorException(string message, string errorCode, ErrorSeverity severity = ErrorSeverity.Error, RecoveryStrategy recoveryStrategy = RecoveryStrategy.None)
        : base(message)
    {
        ErrorCode = errorCode;
        Severity = severity;
        RecoveryStrategy = recoveryStrategy;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowOrchestratorException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="severity">The severity of the error.</param>
    /// <param name="recoveryStrategy">The recovery strategy.</param>
    public FlowOrchestratorException(string message, Exception innerException, string errorCode, ErrorSeverity severity = ErrorSeverity.Error, RecoveryStrategy recoveryStrategy = RecoveryStrategy.None)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Severity = severity;
        RecoveryStrategy = recoveryStrategy;
    }
}

/// <summary>
/// Defines the severity of an error.
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// A warning that does not affect the operation of the system.
    /// </summary>
    Warning,

    /// <summary>
    /// An error that affects the current operation but not the entire system.
    /// </summary>
    Error,

    /// <summary>
    /// A critical error that affects the entire system.
    /// </summary>
    Critical
}

/// <summary>
/// Defines the recovery strategy for an error.
/// </summary>
public enum RecoveryStrategy
{
    /// <summary>
    /// No recovery strategy.
    /// </summary>
    None,

    /// <summary>
    /// Retry the operation.
    /// </summary>
    Retry,

    /// <summary>
    /// Skip the current operation and continue with the next one.
    /// </summary>
    Skip,

    /// <summary>
    /// Use an alternative path.
    /// </summary>
    AlternativePath,

    /// <summary>
    /// Compensate for the error by performing a compensating action.
    /// </summary>
    Compensate,

    /// <summary>
    /// Terminate the operation.
    /// </summary>
    Terminate
}

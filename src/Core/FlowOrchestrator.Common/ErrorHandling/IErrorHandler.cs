namespace FlowOrchestrator.Common.ErrorHandling;

/// <summary>
/// Defines the contract for an error handler.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handles an exception.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>The result of handling the exception.</returns>
    ErrorHandlingResult HandleException(Exception exception);

    /// <summary>
    /// Handles an exception with a specific context.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="context">The context in which the exception occurred.</param>
    /// <returns>The result of handling the exception.</returns>
    ErrorHandlingResult HandleException(Exception exception, object context);
}

/// <summary>
/// Represents the result of handling an error.
/// </summary>
public class ErrorHandlingResult
{
    /// <summary>
    /// Gets a value indicating whether the error was handled successfully.
    /// </summary>
    public bool Handled { get; }

    /// <summary>
    /// Gets the recovery action to take.
    /// </summary>
    public RecoveryAction RecoveryAction { get; }

    /// <summary>
    /// Gets the error details.
    /// </summary>
    public ErrorDetails ErrorDetails { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlingResult"/> class.
    /// </summary>
    /// <param name="handled">A value indicating whether the error was handled successfully.</param>
    /// <param name="recoveryAction">The recovery action to take.</param>
    /// <param name="errorDetails">The error details.</param>
    public ErrorHandlingResult(bool handled, RecoveryAction recoveryAction, ErrorDetails errorDetails)
    {
        Handled = handled;
        RecoveryAction = recoveryAction;
        ErrorDetails = errorDetails;
    }
}

/// <summary>
/// Represents the details of an error.
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the severity of the error.
    /// </summary>
    public ErrorSeverity Severity { get; }

    /// <summary>
    /// Gets the timestamp of the error.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Gets the exception that caused the error.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDetails"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="severity">The severity of the error.</param>
    /// <param name="exception">The exception that caused the error.</param>
    public ErrorDetails(string errorCode, string message, ErrorSeverity severity, Exception? exception = null)
    {
        ErrorCode = errorCode;
        Message = message;
        Severity = severity;
        Timestamp = DateTime.UtcNow;
        Exception = exception;
    }
}

/// <summary>
/// Defines the recovery action to take after handling an error.
/// </summary>
public enum RecoveryAction
{
    /// <summary>
    /// No recovery action.
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

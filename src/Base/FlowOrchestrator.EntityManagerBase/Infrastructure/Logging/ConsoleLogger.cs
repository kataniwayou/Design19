using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.Logging;

/// <summary>
/// Console logger implementation.
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Debug(string message) => Console.WriteLine($"DEBUG: {message}");

    /// <summary>
    /// Logs a debug message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Debug(string message, Exception exception) => Console.WriteLine($"DEBUG: {message} - {exception}");

    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Info(string message) => Console.WriteLine($"INFO: {message}");

    /// <summary>
    /// Logs an information message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Info(string message, Exception exception) => Console.WriteLine($"INFO: {message} - {exception}");

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Warn(string message) => Console.WriteLine($"WARN: {message}");

    /// <summary>
    /// Logs a warning message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Warn(string message, Exception exception) => Console.WriteLine($"WARN: {message} - {exception}");

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Error(string message) => Console.WriteLine($"ERROR: {message}");

    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Error(string message, Exception exception) => Console.WriteLine($"ERROR: {message} - {exception}");

    /// <summary>
    /// Logs a fatal message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Fatal(string message) => Console.WriteLine($"FATAL: {message}");

    /// <summary>
    /// Logs a fatal message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Fatal(string message, Exception exception) => Console.WriteLine($"FATAL: {message} - {exception}");
}

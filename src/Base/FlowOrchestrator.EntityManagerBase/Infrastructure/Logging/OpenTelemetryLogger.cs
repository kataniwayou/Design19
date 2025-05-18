using FlowOrchestrator.Infrastructure.Common.Telemetry;
using ILogger = FlowOrchestrator.Common.Logging.ILogger;

namespace FlowOrchestrator.EntityManagerBase.Infrastructure.Logging;

/// <summary>
/// OpenTelemetry logger implementation that sends logs to the OpenTelemetry collector.
/// </summary>
public class OpenTelemetryLogger : ILogger
{
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly string _serviceName;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetryLogger"/> class.
    /// </summary>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="serviceName">The service name.</param>
    public OpenTelemetryLogger(ITelemetryProvider telemetryProvider, string serviceName)
    {
        _telemetryProvider = telemetryProvider;
        _serviceName = serviceName;
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Debug(string message)
    {
        LogToOpenTelemetry("DEBUG", message, null);
        Console.WriteLine($"DEBUG: {message}");
    }

    /// <summary>
    /// Logs a debug message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Debug(string message, Exception exception)
    {
        LogToOpenTelemetry("DEBUG", message, exception);
        Console.WriteLine($"DEBUG: {message} - {exception}");
    }

    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Info(string message)
    {
        LogToOpenTelemetry("INFO", message, null);
        Console.WriteLine($"INFO: {message}");
    }

    /// <summary>
    /// Logs an information message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Info(string message, Exception exception)
    {
        LogToOpenTelemetry("INFO", message, exception);
        Console.WriteLine($"INFO: {message} - {exception}");
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Warn(string message)
    {
        LogToOpenTelemetry("WARN", message, null);
        Console.WriteLine($"WARN: {message}");
    }

    /// <summary>
    /// Logs a warning message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Warn(string message, Exception exception)
    {
        LogToOpenTelemetry("WARN", message, exception);
        Console.WriteLine($"WARN: {message} - {exception}");
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Error(string message)
    {
        LogToOpenTelemetry("ERROR", message, null);
        Console.WriteLine($"ERROR: {message}");
    }

    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Error(string message, Exception exception)
    {
        LogToOpenTelemetry("ERROR", message, exception);
        Console.WriteLine($"ERROR: {message} - {exception}");
    }

    /// <summary>
    /// Logs a fatal message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Fatal(string message)
    {
        LogToOpenTelemetry("FATAL", message, null);
        Console.WriteLine($"FATAL: {message}");
    }

    /// <summary>
    /// Logs a fatal message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    public void Fatal(string message, Exception exception)
    {
        LogToOpenTelemetry("FATAL", message, exception);
        Console.WriteLine($"FATAL: {message} - {exception}");
    }

    private void LogToOpenTelemetry(string level, string message, Exception? exception)
    {
        var attributes = new Dictionary<string, object>
        {
            { "log.level", level },
            { "service.name", _serviceName },
            { "message", message }
        };

        if (exception != null)
        {
            attributes.Add("exception.type", exception.GetType().FullName ?? "Unknown");
            attributes.Add("exception.message", exception.Message);
            attributes.Add("exception.stacktrace", exception.StackTrace ?? "No stack trace");
        }

        _telemetryProvider.RecordEvent("log", attributes);
    }
}

namespace FlowOrchestrator.Common.Utilities;

/// <summary>
/// Provides utility methods for string operations.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Checks if a string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is null or empty, false otherwise.</returns>
    public static bool IsNullOrEmpty(string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Checks if a string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is null, empty, or consists only of white-space characters, false otherwise.</returns>
    public static bool IsNullOrWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Truncates a string to a specified length.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length of the string.</param>
    /// <param name="suffix">The suffix to append to the truncated string. Default is "...".</param>
    /// <returns>The truncated string.</returns>
    public static string? Truncate(string? value, int maxLength, string suffix = "...")
    {
        if (IsNullOrEmpty(value) || value!.Length <= maxLength)
        {
            return value;
        }

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// Converts a string to a safe identifier by replacing invalid characters with underscores.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The safe identifier.</returns>
    public static string ToSafeIdentifier(string? value)
    {
        if (IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        // Replace invalid characters with underscores
        return System.Text.RegularExpressions.Regex.Replace(value!, @"[^a-zA-Z0-9_]", "_");
    }
}

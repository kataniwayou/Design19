using FlowOrchestrator.Common.Utilities;
using Xunit;

namespace FlowOrchestrator.Common.Tests.Utilities;

public class StringUtilitiesTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("test", false)]
    public void IsNullOrEmpty_ReturnsExpectedResult(string? value, bool expected)
    {
        // Act
        var result = StringUtilities.IsNullOrEmpty(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("\t", true)]
    [InlineData("\n", true)]
    [InlineData("test", false)]
    public void IsNullOrWhiteSpace_ReturnsExpectedResult(string? value, bool expected)
    {
        // Act
        var result = StringUtilities.IsNullOrWhiteSpace(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, 10, null)]
    [InlineData("", 10, "")]
    [InlineData("test", 10, "test")]
    [InlineData("test string", 7, "test...")]
    [InlineData("test string", 8, "test s..", "..")]
    public void Truncate_ReturnsExpectedResult(string? value, int maxLength, string? expected, string suffix = "...")
    {
        // Act
        var result = StringUtilities.Truncate(value, maxLength, suffix);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("test", "test")]
    [InlineData("test string", "test_string")]
    [InlineData("test-string", "test_string")]
    [InlineData("test@string", "test_string")]
    [InlineData("test123", "test123")]
    [InlineData("test_string", "test_string")]
    public void ToSafeIdentifier_ReturnsExpectedResult(string? value, string expected)
    {
        // Act
        var result = StringUtilities.ToSafeIdentifier(value);

        // Assert
        Assert.Equal(expected, result);
    }
}

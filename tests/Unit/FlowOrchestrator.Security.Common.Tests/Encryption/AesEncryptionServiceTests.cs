using FlowOrchestrator.Security.Common.Encryption;
using Xunit;

namespace FlowOrchestrator.Security.Common.Tests.Encryption;

public class AesEncryptionServiceTests
{
    private readonly AesEncryptionService _encryptionService;

    public AesEncryptionServiceTests()
    {
        _encryptionService = new AesEncryptionService("TestKey12345678", "TestIV1234567890");
    }

    [Fact]
    public void Encrypt_WithValidInput_ReturnsEncryptedText()
    {
        // Arrange
        var plainText = "Test plain text";

        // Act
        var encryptedText = _encryptionService.Encrypt(plainText);

        // Assert
        Assert.NotNull(encryptedText);
        Assert.NotEmpty(encryptedText);
        Assert.NotEqual(plainText, encryptedText);
    }

    [Fact]
    public void Decrypt_WithValidInput_ReturnsDecryptedText()
    {
        // Arrange
        var plainText = "Test plain text";
        var encryptedText = _encryptionService.Encrypt(plainText);

        // Act
        var decryptedText = _encryptionService.Decrypt(encryptedText);

        // Assert
        Assert.NotNull(decryptedText);
        Assert.NotEmpty(decryptedText);
        Assert.Equal(plainText, decryptedText);
    }

    [Fact]
    public void Hash_WithValidInput_ReturnsHashedText()
    {
        // Arrange
        var plainText = "Test plain text";

        // Act
        var hashedText = _encryptionService.Hash(plainText);

        // Assert
        Assert.NotNull(hashedText);
        Assert.NotEmpty(hashedText);
        Assert.NotEqual(plainText, hashedText);
    }

    [Fact]
    public void VerifyHash_WithValidInput_ReturnsTrue()
    {
        // Arrange
        var plainText = "Test plain text";
        var hashedText = _encryptionService.Hash(plainText);

        // Act
        var result = _encryptionService.VerifyHash(plainText, hashedText);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyHash_WithInvalidInput_ReturnsFalse()
    {
        // Arrange
        var plainText = "Test plain text";
        var invalidPlainText = "Invalid plain text";
        var hashedText = _encryptionService.Hash(plainText);

        // Act
        var result = _encryptionService.VerifyHash(invalidPlainText, hashedText);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Encrypt_WithEmptyOrNullInput_ReturnsEmptyString(string plainText)
    {
        // Act
        var encryptedText = _encryptionService.Encrypt(plainText);

        // Assert
        Assert.Equal(string.Empty, encryptedText);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Decrypt_WithEmptyOrNullInput_ReturnsEmptyString(string encryptedText)
    {
        // Act
        var decryptedText = _encryptionService.Decrypt(encryptedText);

        // Assert
        Assert.Equal(string.Empty, decryptedText);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Hash_WithEmptyOrNullInput_ReturnsEmptyString(string plainText)
    {
        // Act
        var hashedText = _encryptionService.Hash(plainText);

        // Assert
        Assert.Equal(string.Empty, hashedText);
    }

    [Theory]
    [InlineData("", "hashedText")]
    [InlineData(null, "hashedText")]
    [InlineData("plainText", "")]
    [InlineData("plainText", null)]
    public void VerifyHash_WithEmptyOrNullInput_ReturnsFalse(string plainText, string hashedText)
    {
        // Act
        var result = _encryptionService.VerifyHash(plainText, hashedText);

        // Assert
        Assert.False(result);
    }
}

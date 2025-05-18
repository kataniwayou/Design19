namespace FlowOrchestrator.Security.Common.Encryption;

/// <summary>
/// Defines the contract for an encryption service.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts a string.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted text.</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts a string.
    /// </summary>
    /// <param name="encryptedText">The encrypted text to decrypt.</param>
    /// <returns>The decrypted text.</returns>
    string Decrypt(string encryptedText);

    /// <summary>
    /// Hashes a string.
    /// </summary>
    /// <param name="plainText">The plain text to hash.</param>
    /// <returns>The hashed text.</returns>
    string Hash(string plainText);

    /// <summary>
    /// Verifies a hash.
    /// </summary>
    /// <param name="plainText">The plain text to verify.</param>
    /// <param name="hashedText">The hashed text to verify against.</param>
    /// <returns>True if the hash is valid, false otherwise.</returns>
    bool VerifyHash(string plainText, string hashedText);
}

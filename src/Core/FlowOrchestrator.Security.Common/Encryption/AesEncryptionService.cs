using System.Security.Cryptography;
using System.Text;

namespace FlowOrchestrator.Security.Common.Encryption;

/// <summary>
/// AES implementation of the encryption service.
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    /// <summary>
    /// Initializes a new instance of the <see cref="AesEncryptionService"/> class.
    /// </summary>
    /// <param name="key">The encryption key.</param>
    /// <param name="iv">The initialization vector.</param>
    public AesEncryptionService(string key, string iv)
    {
        // Ensure key and IV are of correct length
        _key = DeriveKeyFromPassword(key, 32); // 256 bits
        _iv = DeriveKeyFromPassword(iv, 16); // 128 bits
    }

    /// <summary>
    /// Encrypts a string.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted text.</returns>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(plainText);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary>
    /// Decrypts a string.
    /// </summary>
    /// <param name="encryptedText">The encrypted text to decrypt.</param>
    /// <returns>The decrypted text.</returns>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            return string.Empty;
        }

        byte[] buffer = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        
        return streamReader.ReadToEnd();
    }

    /// <summary>
    /// Hashes a string.
    /// </summary>
    /// <param name="plainText">The plain text to hash.</param>
    /// <returns>The hashed text.</returns>
    public string Hash(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        // Generate a random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with the salt
        byte[] hash = HashWithSalt(plainText, salt);

        // Combine the salt and hash
        byte[] hashBytes = new byte[salt.Length + hash.Length];
        Array.Copy(salt, 0, hashBytes, 0, salt.Length);
        Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

        // Convert to base64
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifies a hash.
    /// </summary>
    /// <param name="plainText">The plain text to verify.</param>
    /// <param name="hashedText">The hashed text to verify against.</param>
    /// <returns>True if the hash is valid, false otherwise.</returns>
    public bool VerifyHash(string plainText, string hashedText)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(hashedText))
        {
            return false;
        }

        try
        {
            // Convert from base64
            byte[] hashBytes = Convert.FromBase64String(hashedText);

            // Extract the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, salt.Length);

            // Extract the hash
            byte[] hash = new byte[hashBytes.Length - salt.Length];
            Array.Copy(hashBytes, salt.Length, hash, 0, hash.Length);

            // Hash the password with the extracted salt
            byte[] computedHash = HashWithSalt(plainText, salt);

            // Compare the hashes
            return SlowEquals(hash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    private byte[] DeriveKeyFromPassword(string password, int keyLength)
    {
        byte[] salt = Encoding.UTF8.GetBytes("FlowOrchestratorSalt");
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(keyLength);
    }

    private byte[] HashWithSalt(string plainText, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, 10000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32); // 256 bits
    }

    private bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }
}

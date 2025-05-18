namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents parameters for an import operation.
/// </summary>
public class ImportParameters
{
    /// <summary>
    /// Gets or sets the source identifier.
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source address.
    /// </summary>
    public string SourceAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source credentials.
    /// </summary>
    public SourceCredentials? Credentials { get; set; }

    /// <summary>
    /// Gets or sets the import configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Represents credentials for a source.
/// </summary>
public class SourceCredentials
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the certificate path.
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>
    /// Gets or sets the certificate password.
    /// </summary>
    public string? CertificatePassword { get; set; }

    /// <summary>
    /// Gets or sets additional authentication parameters.
    /// </summary>
    public Dictionary<string, string> AdditionalParameters { get; set; } = new Dictionary<string, string>();
}

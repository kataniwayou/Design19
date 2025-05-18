namespace FlowOrchestrator.IntegrationBase.Messaging;

/// <summary>
/// Represents parameters for an export operation.
/// </summary>
public class ExportParameters
{
    /// <summary>
    /// Gets or sets the destination identifier.
    /// </summary>
    public string DestinationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination address.
    /// </summary>
    public string DestinationAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination credentials.
    /// </summary>
    public DestinationCredentials? Credentials { get; set; }

    /// <summary>
    /// Gets or sets the data to export.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the data format.
    /// </summary>
    public string DataFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data schema.
    /// </summary>
    public SchemaDefinition? Schema { get; set; }

    /// <summary>
    /// Gets or sets the export configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Represents credentials for a destination.
/// </summary>
public class DestinationCredentials
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

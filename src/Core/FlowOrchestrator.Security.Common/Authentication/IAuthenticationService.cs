namespace FlowOrchestrator.Security.Common.Authentication;

/// <summary>
/// Defines the contract for an authentication service.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>The authentication result.</returns>
    Task<AuthenticationResult> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Validates a token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>The validation result.</returns>
    Task<TokenValidationResult> ValidateTokenAsync(string token);

    /// <summary>
    /// Refreshes a token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>The refresh result.</returns>
    Task<TokenRefreshResult> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes a token.
    /// </summary>
    /// <param name="token">The token to revoke.</param>
    /// <returns>True if the token was revoked, false otherwise.</returns>
    Task<bool> RevokeTokenAsync(string token);
}

/// <summary>
/// Represents the result of an authentication operation.
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// Gets a value indicating whether the authentication was successful.
    /// </summary>
    public bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the access token.
    /// </summary>
    public string? AccessToken { get; }

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    public string? RefreshToken { get; }

    /// <summary>
    /// Gets the expiration time of the access token.
    /// </summary>
    public DateTime? ExpiresAt { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationResult"/> class for a successful authentication.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="expiresAt">The expiration time of the access token.</param>
    public AuthenticationResult(string accessToken, string refreshToken, DateTime expiresAt)
    {
        IsAuthenticated = true;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationResult"/> class for a failed authentication.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public AuthenticationResult(string errorMessage)
    {
        IsAuthenticated = false;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Represents the result of a token validation operation.
/// </summary>
public class TokenValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the token is valid.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Gets the username.
    /// </summary>
    public string? Username { get; }

    /// <summary>
    /// Gets the roles.
    /// </summary>
    public List<string>? Roles { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationResult"/> class for a valid token.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="username">The username.</param>
    /// <param name="roles">The roles.</param>
    public TokenValidationResult(string userId, string username, List<string> roles)
    {
        IsValid = true;
        UserId = userId;
        Username = username;
        Roles = roles;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationResult"/> class for an invalid token.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public TokenValidationResult(string errorMessage)
    {
        IsValid = false;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Represents the result of a token refresh operation.
/// </summary>
public class TokenRefreshResult
{
    /// <summary>
    /// Gets a value indicating whether the refresh was successful.
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// Gets the access token.
    /// </summary>
    public string? AccessToken { get; }

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    public string? RefreshToken { get; }

    /// <summary>
    /// Gets the expiration time of the access token.
    /// </summary>
    public DateTime? ExpiresAt { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRefreshResult"/> class for a successful refresh.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="expiresAt">The expiration time of the access token.</param>
    public TokenRefreshResult(string accessToken, string refreshToken, DateTime expiresAt)
    {
        IsSuccessful = true;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRefreshResult"/> class for a failed refresh.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public TokenRefreshResult(string errorMessage)
    {
        IsSuccessful = false;
        ErrorMessage = errorMessage;
    }
}

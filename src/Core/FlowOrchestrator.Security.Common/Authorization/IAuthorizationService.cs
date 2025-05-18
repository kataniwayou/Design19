namespace FlowOrchestrator.Security.Common.Authorization;

/// <summary>
/// Defines the contract for an authorization service.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Authorizes a user for a resource.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="resource">The resource to authorize.</param>
    /// <param name="action">The action to authorize.</param>
    /// <returns>The authorization result.</returns>
    Task<AuthorizationResult> AuthorizeAsync(string userId, string resource, string action);

    /// <summary>
    /// Authorizes a user for a resource.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="resource">The resource to authorize.</param>
    /// <param name="action">The action to authorize.</param>
    /// <param name="resourceId">The resource identifier.</param>
    /// <returns>The authorization result.</returns>
    Task<AuthorizationResult> AuthorizeAsync(string userId, string resource, string action, string resourceId);

    /// <summary>
    /// Gets the permissions for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The permissions.</returns>
    Task<IEnumerable<Permission>> GetPermissionsAsync(string userId);

    /// <summary>
    /// Gets the roles for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The roles.</returns>
    Task<IEnumerable<string>> GetRolesAsync(string userId);
}

/// <summary>
/// Represents the result of an authorization operation.
/// </summary>
public class AuthorizationResult
{
    /// <summary>
    /// Gets a value indicating whether the authorization was successful.
    /// </summary>
    public bool IsAuthorized { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationResult"/> class for a successful authorization.
    /// </summary>
    public AuthorizationResult()
    {
        IsAuthorized = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationResult"/> class for a failed authorization.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public AuthorizationResult(string errorMessage)
    {
        IsAuthorized = false;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Represents a permission.
/// </summary>
public class Permission
{
    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the action.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Permission"/> class.
    /// </summary>
    public Permission()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Permission"/> class.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <param name="action">The action.</param>
    /// <param name="resourceId">The resource identifier.</param>
    public Permission(string resource, string action, string? resourceId = null)
    {
        Resource = resource;
        Action = action;
        ResourceId = resourceId;
    }
}

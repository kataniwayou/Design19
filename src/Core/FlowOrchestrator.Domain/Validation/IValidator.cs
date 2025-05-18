namespace FlowOrchestrator.Domain.Validation;

/// <summary>
/// Defines the contract for a validator.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the specified object.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>The validation result.</returns>
    ValidationResult Validate(T obj);
}

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public List<ValidationError> Errors { get; } = new List<ValidationError>();

    /// <summary>
    /// Adds an error to the validation result.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorMessage">The error message.</param>
    public void AddError(string propertyName, string errorMessage)
    {
        Errors.Add(new ValidationError(propertyName, errorMessage));
    }

    /// <summary>
    /// Adds an error to the validation result.
    /// </summary>
    /// <param name="error">The validation error.</param>
    public void AddError(ValidationError error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Adds errors from another validation result.
    /// </summary>
    /// <param name="result">The validation result to add errors from.</param>
    public void AddErrors(ValidationResult result)
    {
        Errors.AddRange(result.Errors);
    }
}

/// <summary>
/// Represents a validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets the name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}

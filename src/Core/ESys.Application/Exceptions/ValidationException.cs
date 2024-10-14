using FluentValidation.Results;

namespace ESys.Application.Exceptions;

/// <summary>
/// Occurs when data provided by client have validation issues. Pipeline returns BadRequest with status code 400
/// </summary>
public class ValidationException : Exception
{
    public List<string> ValdationErrors { get; set; }

    public ValidationException(ValidationResult validationResult)
    {
        ValdationErrors = new List<string>();

        foreach (var validationError in validationResult.Errors)
        {
            ValdationErrors.Add(validationError.ErrorMessage);
        }
    }
}
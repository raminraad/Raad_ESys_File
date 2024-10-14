namespace ESys.Application.Exceptions;

/// <summary>
/// Occurs when data received from client are not valid. Pipeline returns BadRequest with status code 400
/// </summary>
public class BadRequestException: Exception
{
    public BadRequestException(string message): base(message)
    {

    }
}
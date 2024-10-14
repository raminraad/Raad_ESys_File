namespace ESys.Application.Exceptions;

/// <summary>
/// Occurs when data with provided key is not found in database. Pipeline returns NotFound with status code 404
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"{name} ({key}) was not found on server.")
    {
    }
}
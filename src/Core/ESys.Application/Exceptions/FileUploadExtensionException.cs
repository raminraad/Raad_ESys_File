using Microsoft.AspNetCore.Http;

namespace ESys.Application.Exceptions;

/// <summary>
/// Occurs when file being uploaded by client isn't included in defined extensions
/// </summary>
public class FileUploadExtensionException: Exception
{
    public FileUploadExtensionException(string message): base(message)
    {

    }
    
    public FileUploadExtensionException(IFormFile file): base($"File {file.Name} extension doesn't match with defined patterns.")
    {

    }
}
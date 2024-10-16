using Microsoft.AspNetCore.Http;

namespace ESys.Application.Exceptions;

/// <summary>
/// Occurs when file being uploaded by client exceeds size limit
/// </summary>
public class FileUploadSizeLimitException: Exception
{
    public FileUploadSizeLimitException(string message): base(message)
    {

    }
    
    public FileUploadSizeLimitException(IFormFile file): base($"File {file.Name} exceeds size limit.")
    {

    }
    
    public FileUploadSizeLimitException(IFormFile file,int sizeLimitInMB): base($"File {file.Name} exceeds size limit of {sizeLimitInMB} MB.")
    {

    }
}
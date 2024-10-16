using ESys.Application.Exceptions;
using ESys.Application.Services.FileHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ESys.Persistence.FileSystem;

public class UploadHandlerService : IUploadHandlerService
{
    private readonly UploadHandlerConfig _uploadHandlerConfig;

    public UploadHandlerService(IConfiguration configuration)
    {
        _uploadHandlerConfig = configuration.GetSection("UploadHandlerConfig") as UploadHandlerConfig;
    }

    private bool IsFileValid(IFormFile file)
    {
        //Checking the file for extension validation
        var fileExtension = Path.GetExtension(file.FileName);
        if (_uploadHandlerConfig.AcceptedExtensions is not null &&
            !_uploadHandlerConfig.AcceptedExtensions.Contains(fileExtension))
        {
            throw new FileUploadExtensionException(file);
        }

        //Checking the file for size validation
        long size = file.Length;
        var sizeLimit = _uploadHandlerConfig.MaxSizeInMB * 1024 * 1024;
        if (size > sizeLimit)
        {
            throw new FileUploadSizeLimitException(file, sizeLimit);
        }

        return true;
    }

    public string Upload(IFormFile file)
    {
        if (!IsFileValid(file))
            return "File(s) not valid.";

        //name changing
        var fileExtension = Path.GetExtension(file.FileName);
        var fileNewName = Guid.NewGuid().ToString() + fileExtension;
        var path = Path.Combine(Directory.GetCurrentDirectory(),
            _uploadHandlerConfig.UploadRootDirectory );
        using var stream = new FileStream(Path.Combine(path, fileNewName), FileMode.Create);
        file.CopyTo(stream);
        return fileNewName;
    }

    public string Upload(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            Upload(file);
        }

        return "Uploads succeeded.";
    }
}
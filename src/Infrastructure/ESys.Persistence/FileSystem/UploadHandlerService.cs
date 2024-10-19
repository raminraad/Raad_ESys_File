using ESys.Application.Exceptions;
using ESys.Application.Services.FileHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ESys.Persistence.FileSystem;

public class UploadHandlerService : IUploadHandlerService
{
    private UploadHandlerConfig _uploadHandlerConfig;

    public UploadHandlerConfig UploadHandlerConfig
    {
        get => _uploadHandlerConfig;
        set => _uploadHandlerConfig = value;
    }

    public UploadHandlerService(IConfiguration configuration)
    {
        _uploadHandlerConfig = configuration.GetSection("UploadHandlerConfig").Get<UploadHandlerConfig>();
    }

    private bool IsFileValid(IFormFile file)
    {
        //Checking the file for extension validation
        var fileExtension = Path.GetExtension(file.FileName);
        if (_uploadHandlerConfig.AcceptedExtensions.Count > 0 &&
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
        var path = Path.Combine(Directory.GetCurrentDirectory(),
            _uploadHandlerConfig.UploadRootDirectory, _uploadHandlerConfig.UploadChildDirectory);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var fileNewName = file.FileName.Remove(file.FileName.Length - fileExtension.Length);
        while (File.Exists(Path.Combine(path, fileNewName + fileExtension)))
        {
            fileNewName += "_";
        }

        using var stream = new FileStream(Path.Combine(path, fileNewName + fileExtension), FileMode.Create);
        file.CopyTo(stream);
        return fileNewName + fileExtension;
    }

    public IEnumerable<string> Upload(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
            yield return Upload(file);
    }
}
using Microsoft.AspNetCore.Http;

namespace ESys.Application.Services.FileHandler;

public interface IUploadHandlerService
{
    public string Upload(IFormFile file);
    public string Upload(IEnumerable<IFormFile> files);

}
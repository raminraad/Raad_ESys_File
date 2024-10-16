using Microsoft.AspNetCore.Http;

namespace ESys.Application.Services.FileHandler;

public interface IUploadHandlerService
{
    UploadHandlerConfig UploadHandlerConfig { set; get; }
    string Upload(IFormFile file);
    string Upload(IEnumerable<IFormFile> files);

}
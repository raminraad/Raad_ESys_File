using ESys.Persistence.FileSystem;
using Microsoft.AspNetCore.Mvc;

namespace ESys.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class FileController : ControllerBase
{
    private readonly UploadHandlerService _uploadHandlerService;

    public FileController(UploadHandlerService _uploadHandlerService)
    {
        this._uploadHandlerService = _uploadHandlerService;
    }
    
    [HttpPost(Name = "Upload")]
    public IActionResult Upload(IFormFile file)
    {
        return Ok(_uploadHandlerService.Upload(file));
    }
    
    [HttpPost(Name = "UploadMultiple")]
    public IActionResult UploadMultiple(IEnumerable<IFormFile> files)
    {
        return Ok(_uploadHandlerService.Upload(files));
    }
}
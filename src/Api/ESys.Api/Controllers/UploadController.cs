using ESys.Application.Services.FileHandler;
using Microsoft.AspNetCore.Mvc;

namespace ESys.Api.Controllers;

[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly IUploadHandlerService _uploadHandlerService;
    private readonly UploadHandlerConfig _uploadHandlerConfig;

    public UploadController(IUploadHandlerService _uploadHandlerService,IConfiguration configuration)
    {
        this._uploadHandlerService = _uploadHandlerService;
        var uploadHandlerConfig = configuration.GetSection("UploadHandlerConfig").Get<UploadHandlerConfig>() ;
        
        _uploadHandlerConfig = uploadHandlerConfig;
    }
    
    [HttpPost()]
    [Route("biz/single/{bizId}/{orderId}")]
    public IActionResult BizUploadSingle(IFormFile file,string bizId,string orderId)
    {
        try
        {
            _uploadHandlerService.UploadHandlerConfig.UploadChildDirectory = $"Biz\\{bizId}\\{orderId}";
            return Ok(_uploadHandlerService.Upload(file));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpPost()]
    [Route("biz/multiple/{bizId}/{orderId}")]
    public IActionResult BizUploadMultiple(IEnumerable<IFormFile> files,string bizId,string orderId)
    {
        try
        {
            _uploadHandlerService.UploadHandlerConfig.UploadChildDirectory = $"Biz\\{bizId}\\{orderId}";
            return Ok(_uploadHandlerService.Upload(files));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
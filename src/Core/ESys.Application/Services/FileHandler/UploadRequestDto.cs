using Microsoft.AspNetCore.Http;

namespace ESys.Application.Services.FileHandler;

public class UploadRequestDto
{
    public IFormFile File { get; set; }
}
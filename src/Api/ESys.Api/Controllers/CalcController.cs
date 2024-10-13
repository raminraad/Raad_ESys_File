using ESys.Application.Contracts.Persistence;
using ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class CalcController : Controller
{
    private readonly IMediator _mediator;
    public CalcController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("dyna")]
    public async Task<IActionResult> GetCalculatedBizForm([FromBody] GetCalculatedBizFormQuery request)
    {
        var response = await _mediator.Send(request);
        return Ok(response.Result);
    }
    
    [HttpGet]
    [Route("dyna/{bizId}")]
    public async Task<IActionResult> GetInitiatedBizForm(GetCalculatedBizFormQuery request)
    {
        var response = await _mediator.Send(request);
        return Ok(response.Result);
    }
}

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
    [Route("Dyna")]
    public async Task<IActionResult> GetCalculateFromInitializationValues(GetCalculatedBizFromQuery request)
    {
        var response = await _mediator.Send(request);
        return Ok(response.Result);
    }
}

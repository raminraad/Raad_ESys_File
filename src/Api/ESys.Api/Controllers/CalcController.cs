using ESys.Application.Contracts.Persistence;
using ESys.Application.Features.CalcForm.Queries.GetCalcFormInitialData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class CalcController : Controller
{
    private readonly IExpressionRepository _expressionRepository;
    private readonly GetCalcFormInitialDataQueryHandler _calculationFormInitiator;
    private readonly IMediator _mediator;
    public CalcController(IExpressionRepository expressionRepository, GetCalcFormInitialDataQueryHandler calculationFormInitiator, IMediator mediator)
    {
        _mediator = mediator;
        _calculationFormInitiator = calculationFormInitiator;
        _expressionRepository = expressionRepository;
    }

    [HttpPost]
    [Route("Dyna")]
    public async Task<IActionResult> GetCalculateFromInitializationValues(GetCalcFormInitialDataQuery request)
    {
        var response = await _mediator.Send(request);
        return Ok(response.Result);
    }
}

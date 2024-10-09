using ESys.Api.TempForRevision;
using ESys.Application.Contracts.Persistence;
using ESys.Application.Features;
using ESys.Application.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class CalcController : Controller
{
    private readonly IExpressionRepository _expressionRepository;
    private readonly CalculationFormInitiator _calculationFormInitiator;
    public CalcController(IExpressionRepository expressionRepository, CalculationFormInitiator calculationFormInitiator)
    {
        _calculationFormInitiator = calculationFormInitiator;
        _expressionRepository = expressionRepository;
    }

    [HttpPost]
    [Route("Dyna")]
    public IActionResult GetCalculateFromInitializationValues(IEnumerable<CalculateFromInitializationRequest> requests)
    {
        var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(requests);
        var temp = _calculationFormInitiator.DynaCalcByExp(requestJson);
        return Ok(temp);
    }
}

using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

/// <summary>
/// Request handler for BizForm calculation. This class is used by mediator
/// </summary>
/// <param name="bizFormCalculator">Calculation provider gotten through dependency injection</param>
public class GetCalculatedBizFormQueryHandler(BizFormCalculator bizFormCalculator)
    : IRequestHandler<GetCalculatedBizFormQuery, GetCalculatedBizFormQueryResponse>
{
    /// <summary>
    /// Base method of request handling which gets run automatically by mediator
    /// </summary>
    /// <param name="request">Request gotten from end point containing data needed for calculation service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Corresponding response containing calculation data</returns>
    public Task<GetCalculatedBizFormQueryResponse> Handle(GetCalculatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetCalculatedBizFormQueryResponse()
        {
            Result = bizFormCalculator.GetCalculatedBizForm(request.Body).Result
        };
        return Task.FromResult(result);
    }

}
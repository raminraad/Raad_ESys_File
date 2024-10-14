using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;

/// <summary>
/// Request handler for BizForm initialization. This class is used by mediator
/// </summary>
/// <param name="bizFormInitiator">initialization provider gotten through dependency injection</param>
public class GetInitiatedBizFormQueryHandler(BizFormInitiator bizFormInitiator)
    : IRequestHandler<GetInitiatedBizFormQuery, GetInitiatedBizFormQueryResponse>
{
    /// <summary>
    /// Base method of request handling which gets run automatically by mediator
    /// </summary>
    /// <param name="request">Request gotten from end point containing data needed for initialization service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Corresponding response containing initialization data</returns>
    public Task<GetInitiatedBizFormQueryResponse> Handle(GetInitiatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetInitiatedBizFormQueryResponse()
        {
            Result = bizFormInitiator.GetInitialBizForm(request.BizId).Result
        };
        return Task.FromResult(result);
    }
}
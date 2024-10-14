using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;

public class GetInitiatedBizFormQueryHandler(BizFormInitiator bizFormInitiator)
    : IRequestHandler<GetInitiatedBizFormQuery, GetInitiatedBizFormQueryResponse>
{
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
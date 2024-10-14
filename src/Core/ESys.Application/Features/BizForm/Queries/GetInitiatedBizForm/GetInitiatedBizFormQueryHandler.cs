using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;

public class GetInitiatedBizFormQueryHandler(BizFormInitiator bizFormInitiator)
    : IRequestHandler<GetInitiatedBizFormQuery, GetInitiatedBizFormResponse>
{
    public Task<GetInitiatedBizFormResponse> Handle(GetInitiatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetInitiatedBizFormResponse()
        {
            Result = bizFormInitiator.GetInitialBizForm(request.BizId).Result
        };
        return Task.FromResult(result);
    }
}
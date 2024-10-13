using MediatR;

namespace ESys.Application.Features.BizCalcForm.Queries.GetInitiatedBizForm;

public class GetInitiatedBizFormQueryHandler(BizInitiator bizInitiator)
    : IRequestHandler<GetInitiatedBizFormQuery, GetInitiatedBizFormVm>
{
    public Task<GetInitiatedBizFormVm> Handle(GetInitiatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetInitiatedBizFormVm()
        {
            Result = bizInitiator.GetInitialBizForm(request.BizId).Result
        };
        return Task.FromResult(result);
    }
}
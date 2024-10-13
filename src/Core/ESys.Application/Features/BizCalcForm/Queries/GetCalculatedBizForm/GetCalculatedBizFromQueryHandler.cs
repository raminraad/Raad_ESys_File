using MediatR;

namespace ESys.Application.Features.BizCalcForm.Queries.GetCalculatedBizForm;

public class GetCalculatedBizFromQueryHandler(CalcForm.Calculation.BizCalculator bizCalculator)
    : IRequestHandler<GetCalculatedBizFromQuery, GetCalculatedBizFromVm>
{
    public Task<GetCalculatedBizFromVm> Handle(GetCalculatedBizFromQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetCalculatedBizFromVm()
        {
            Result = bizCalculator.GetCalculatedBizForm(request.Body).Result
        };
        return Task.FromResult(result);
    }

}
using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

public class GetCalculatedBizFromQueryHandler(BizCalculator bizCalculator)
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
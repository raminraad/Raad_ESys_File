using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

public class GetCalculatedBizFormQueryHandler(BizCalculator bizCalculator)
    : IRequestHandler<GetCalculatedBizFormQuery, GetCalculatedBizFormVm>
{
    public Task<GetCalculatedBizFormVm> Handle(GetCalculatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetCalculatedBizFormVm()
        {
            Result = bizCalculator.GetCalculatedBizForm(request.Body).Result
        };
        return Task.FromResult(result);
    }

}
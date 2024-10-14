using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

public class GetCalculatedBizFormQueryHandler(BizFormCalculator bizFormCalculator)
    : IRequestHandler<GetCalculatedBizFormQuery, GetCalculatedBizFormResponse>
{
    public Task<GetCalculatedBizFormResponse> Handle(GetCalculatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetCalculatedBizFormResponse()
        {
            Result = bizFormCalculator.GetCalculatedBizForm(request.Body).Result
        };
        return Task.FromResult(result);
    }

}
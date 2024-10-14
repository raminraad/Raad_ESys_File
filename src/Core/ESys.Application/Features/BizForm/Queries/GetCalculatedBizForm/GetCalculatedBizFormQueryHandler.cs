using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

public class GetCalculatedBizFormQueryHandler(BizCalculator bizCalculator)
    : IRequestHandler<GetCalculatedBizFormQuery, GetCalculatedBizFormResponse>
{
    public Task<GetCalculatedBizFormResponse> Handle(GetCalculatedBizFormQuery request,
        CancellationToken cancellationToken)
    {
        var result = new GetCalculatedBizFormResponse()
        {
            Result = bizCalculator.GetCalculatedBizForm(request.Body).Result
        };
        return Task.FromResult(result);
    }

}
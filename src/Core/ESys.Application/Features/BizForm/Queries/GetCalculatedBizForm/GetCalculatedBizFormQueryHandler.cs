using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

public class GetCalculatedBizFormQueryHandler(BizFormCalculator bizFormCalculator)
    : IRequestHandler<GetCalculatedBizFormQuery, GetCalculatedBizFormQueryResponse>
{
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
using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
public class GetCalculatedBizFormQuery : IRequest<GetCalculatedBizFormResponse>
{
    // Api request body as JSON
    public string Body { set; get; } = string.Empty;
}

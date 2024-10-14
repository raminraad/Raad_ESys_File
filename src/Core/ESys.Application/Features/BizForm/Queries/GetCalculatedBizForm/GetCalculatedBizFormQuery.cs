using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
public class GetCalculatedBizFormQuery : IRequest<GetCalculatedBizFormQueryResponse>
{
    // Api request body as Json string
    public string Body { set; get; } = string.Empty;
}

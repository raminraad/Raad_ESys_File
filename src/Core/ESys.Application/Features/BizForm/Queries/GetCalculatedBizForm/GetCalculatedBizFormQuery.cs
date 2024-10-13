using FastEndpoints;
using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
public class GetCalculatedBizFormQuery : IRequest<GetCalculatedBizFormVm>
{
    // Api request body as JSON
    [FromBody]
    public string Body { set; get; } = string.Empty;
}

using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;
public class GetCalculatedBizFromQuery : IRequest<GetCalculatedBizFromVm>
{
    // Api request body as JSON
    public string Body { set; get; } = string.Empty;
}

using MediatR;

namespace ESys.Application.Features.BizCalcForm.Queries.GetCalculatedBizForm;
public class GetCalculatedBizFromQuery : IRequest<GetCalculatedBizFromVm>
{
    // Api request body as JSON
    public string Body { set; get; } = string.Empty;
}

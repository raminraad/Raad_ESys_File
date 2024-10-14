using MediatR;

namespace ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;
public class GetInitiatedBizFormQuery : IRequest<GetInitiatedBizFormResponse>
{
    public string BizId { set; get; } = string.Empty;
}

using MediatR;

namespace ESys.Application.Features.BizCalcForm.Queries.GetInitiatedBizForm;
public class GetInitiatedBizFormQuery : IRequest<GetInitiatedBizFormVm>
{
    public string BizId { set; get; } = string.Empty;
}

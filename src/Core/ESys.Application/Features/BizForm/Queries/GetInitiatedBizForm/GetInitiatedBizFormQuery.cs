using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;
public class GetInitiatedBizFormQuery : IRequest<GetInitiatedBizFormVm>
{
    [FromRoute]
    public string BizId { set; get; } = string.Empty;
}

using MediatR;

namespace ESys.Application.Features.CalcForm.Queries.GetCalcFormInitialData;
public class GetCalcFormInitialDataQuery : IRequest<CalcFormInitialDataVm>
{
    public List<BizData> BizList { get; set; } = new();
}

public class BizData
{
    public ValDto BizJson { get; set; } = new();
}

public class ValDto
{
    public string Val { get; set; } = string.Empty;
}

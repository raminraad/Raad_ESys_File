namespace ESys.Domain.Entities;

public class Biz
{
    public string BizId { set; get; } = string.Empty;
    public string Exp { set; get; } = string.Empty;
    public string Func { set; get; } = string.Empty;
    public string Lookup { set; get; } = string.Empty;
    public int State { set; get; }
    public string Name { set; get; } = string.Empty;
    public List<BizXmls> BizXmls { get; set; } = [];
    public UI? UI { get; set; } = default;
}


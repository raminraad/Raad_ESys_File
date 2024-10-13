namespace ESys.Domain.Entities;

public class BizInitialUi
{
    public string BizId { set; get; } = string.Empty;
    public string UiJson { set; get; } = string.Empty;
    public Biz? Biz { get; set; } = default;

}


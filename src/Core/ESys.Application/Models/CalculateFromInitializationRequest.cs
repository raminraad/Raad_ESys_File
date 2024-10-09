namespace ESys.Application.Models;

public class CalculateFromInitializationRequest
{
    public ValueContainer BizJson { get; set; } = new();
}

public class ValueContainer
{
    public string Val { get; set; } = string.Empty;
}

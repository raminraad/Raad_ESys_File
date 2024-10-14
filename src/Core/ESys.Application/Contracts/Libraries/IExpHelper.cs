namespace ESys.Application.Contracts.Libraries;

public interface IExpHelper
{
    Dictionary<string, string> ApplyExpsOnData(Dictionary<string, string> dataDic, Dictionary<string, string> expDic);
}
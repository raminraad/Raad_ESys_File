namespace ESys.Application.Contracts.Libraries;

public interface IExpHelper
{
    Dictionary<string, string> MergeExpAndData(Dictionary<string, string> dataDic, Dictionary<string, string> expDic);
}
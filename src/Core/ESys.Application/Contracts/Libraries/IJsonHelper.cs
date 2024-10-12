namespace ESys.Application.Contracts.Libraries;

public interface IJsonHelper
{
    string ConvertKeyValuePairsToJson(Dictionary<string,string> keyValuePairs);
}
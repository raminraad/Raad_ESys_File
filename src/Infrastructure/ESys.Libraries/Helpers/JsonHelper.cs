using ESys.Application.Contracts.Libraries;

namespace ESys.Libraries.Helpers;

public class JsonHelper : IJsonHelper
{
    public string ConvertKeyValuePairsToJson(Dictionary<string, string> keyValPairs)
    {
        string json = "[{";

        var first = true;

        foreach (var item in keyValPairs)
        {
            if (first)
            {
                json = json + "\"" + item.Key + "\":{\"val\":\"" + item.Value + "\"}";
                first = false;
            }
            else
            {
                json = json + ",\"" + item.Key + "\":{\"val\":\"" + item.Value + "\"}";
            }

        }

        json += "}]";

        return json;
    }
}
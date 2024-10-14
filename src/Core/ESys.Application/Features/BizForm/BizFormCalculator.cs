using System.Numerics;
using ESys.Application.Contracts.Libraries;
using ESys.Application.Contracts.Persistence;
using ESys.Application.Exceptions;
using ESys.Domain.Entities;
using Newtonsoft.Json.Linq;
using Expression = org.matheval.Expression;

namespace ESys.Application.Features.BizForm;

public class BizFormCalculator
{
    IBizRepository _bizRepository;
    private readonly IJsonHelper _jsonHelper;
    private readonly IExpHelper _expHelper;
    private readonly IBizXmlRepository _bizXmlRepository;

    //Q? what is Exp for?
    private string Exp = @"len,*,wid,*,qty,*,qty";
    
    private Dictionary<string, string> dataPool = new();
    private Dictionary<string, string> expPool = new();
    private Dictionary<string, string> funcPool = new();
    private Dictionary<string, string> lookupPool = new();
    private Biz _biz = new();
    

    public BizFormCalculator(IBizRepository bizRepository, IJsonHelper jsonHelper, IExpHelper expHelper, IBizXmlRepository bizXmlRepository)
    {
        _bizXmlRepository = bizXmlRepository;
        _bizRepository = bizRepository;
        _jsonHelper = jsonHelper;
        _expHelper = expHelper;
    }

    public async Task<string> GetCalculatedBizForm(string requestBody)
    {
        // Q?
        // todo: make calculation from string exp and data then return the calculated values.

        FillJsonInDataPool(requestBody);

        var biz = await _bizRepository.GetByIdAsync(BigInteger.Parse(dataPool["bizid"]));
        if (biz?.BizId is not null)
            _biz = biz;
        else
            throw new KeyNotFoundException("Business does not exist.");

        AddExpsToDataPool(_biz);
        AddFuncsToDataPool(_biz);
        AddLookupsToDataPool(_biz);

        foreach (var item in lookupPool)
            ApplyLookups(item.Value);
        foreach (var item in funcPool)
            ApplyFuncs(item.Value);

        var result = _jsonHelper.ConvertKeyValuePairsToJson(_expHelper.ApplyExpsOnData(dataPool, expPool));
        return result;
    }

    private void ApplyLookups(string lookupStr)
    {
        var lookupDic = new Dictionary<string, string>();
        Expression expression = new Expression();

        foreach (var item in dataPool)
            if (double.TryParse(item.Value, out double d) && !Double.IsNaN(d) && !Double.IsInfinity(d))
                expression.Bind(item.Key, d);
            else
                expression.Bind(item.Key, item.Value);

        var lookups = JArray.Parse(lookupStr);
        foreach (JObject root in lookups)
        foreach (KeyValuePair<string, JToken> param in root)
        {
            if ((string)param.Value[BizFormStatics.ExpTag] != null)
            {
                string strFormula = param.Value[BizFormStatics.ExpTag]?.ToString().Replace("\\\"", "'");
                expression.SetFomular(strFormula);
                try
                {
                    lookupDic.Add(param.Key, expression.Eval().ToString());
                }
                catch
                {
                    // ignored
                }

                break; // This break is for preventing duplication
            }

            lookupDic.Add(param.Key, (string)param.Value["val"]);
            break; // This break is for preventing duplication
        }

        var xmlData = _bizXmlRepository.GetBizXmlAsDictionary(_biz, lookupDic);
        foreach (var (key, value) in xmlData.Where(kv => !dataPool.ContainsKey(kv.Key)))
            dataPool.Add(key, value);
    }

    /// <summary>
    /// input type: {{'BizId':{'val':'xxx'}},{'len':{'val':'xxx'}},{'wid':{'val':'xxx'}}}{{'BizId':{'val':'yyy'}},{'len':{'val':'xxx'}},{'wid':{'val':'xxx'}}}
    /// The method will call a biz by BizId and fill the values to get the return/s of the biz.
    /// the format of calling a function is same as a client call like: "{{'BizId':{'val':'xxx'}},{'len':{'val':'xxx'}},{'wid':{'val':'xxx'}}}"
    /// return type of the func will be pair of (key,value) which will add to a dictionary and return by the method.
    /// </summary>
    /// <param name="funcStr"></param>
    /// <returns>Dictionary<key:string, value:string></returns>
    public void ApplyFuncs(string funcStr)
    {
        var keyValuePairs = new Dictionary<string, string>();

        Expression expression = new Expression();
        foreach (var item in dataPool)
        {
            try
            {
                expression.Bind(item.Key, Convert.ToDouble(item.Value));
            }
            catch (Exception)
            {
                expression.Bind(item.Key, item.Value);
                //throw;
            }
        }

        var objects = JArray.Parse(funcStr); // parse as array
        foreach (JObject root in objects)
        {
            foreach (KeyValuePair<string, JToken> param in root)
            {
                if ((string)param.Value[BizFormStatics.ExpTag] != null)
                {
                    expression.SetFomular(new string((string)param.Value[BizFormStatics.ExpTag]).Replace("\\\"", "\""));

                    keyValuePairs.Add(param.Key, expression.Eval().ToString());
                    break;
                }
                else
                {
                    keyValuePairs.Add(param.Key, (string)param.Value["val"]);
                }
            }
        }

        FillJsonInDataPool(GetCalculatedBizForm(_jsonHelper.ConvertKeyValuePairsToJson(keyValuePairs)).Result);
    }

    private void AddExpsToDataPool(Biz biz)
    {
        foreach (JObject root in JArray.Parse(biz.Exp))
        foreach (KeyValuePair<string, JToken> param in root)
            expPool.Add(param.Key, (string)param.Value[BizFormStatics.ExpTag]);
    }

    private void AddFuncsToDataPool(Biz biz)
    {
        try
        {
            if (string.IsNullOrEmpty(biz.Func)) return;
        
            foreach (JObject root in JArray.Parse(biz.Func))
            foreach (KeyValuePair<string, JToken> param in root)
                funcPool.Add(param.Key, param.Value[BizFormStatics.FuncTag].ToString());
        }
        catch (Exception e)
        {
            throw new BadRequestException(e.Message);
        }
    }

    private void AddLookupsToDataPool(Biz biz)
    {
        if (string.IsNullOrEmpty(_biz.Lookup)) return;

        foreach (JObject root in JArray.Parse(biz.Lookup))
        foreach (KeyValuePair<string, JToken> param in root)
            lookupPool.Add(param.Key, (string)param.Value[BizFormStatics.LookupTag]);
    }

    private void FillJsonInDataPool(string jsonData)
    {
        foreach (JObject root in JArray.Parse(jsonData))
        foreach (KeyValuePair<string, JToken> param in root)
            dataPool.Add(param.Key, (string)param.Value["val"]);
    }
}
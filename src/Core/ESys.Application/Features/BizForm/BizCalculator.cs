using System.Numerics;
using ESys.Application.Contracts.Libraries;
using ESys.Application.Contracts.Persistence;
using ESys.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Expression = org.matheval.Expression;


namespace ESys.Application.Features.BizForm;

public class BizCalculator
{
    IBizRepository _bizRepository;
    private readonly IJsonHelper _jsonHelper;
    private readonly IExpHelper _expHelper;

    private readonly IBizXmlRepository _bizXmlRepository;

    private const string EXP = "exp";
    private const string LOOKUP = "lookup";
    private const string FUNC = "func";
    
    //Q? what is Exp for?
    private string Exp = @"len,*,wid,*,qty,*,qty";
    
    private Dictionary<string, string> dataPool = new Dictionary<string, string>();
    private Dictionary<string, string> expPool = new Dictionary<string, string>();
    private Dictionary<string, string> funcPool = new Dictionary<string, string>();
    private Dictionary<string, string> lookupPool = new Dictionary<string, string>();

    private Biz _biz = new();
    private string xmlTable = "dbo.tblBizXmls";
    
    public BizCalculator(IBizRepository bizRepository, IJsonHelper jsonHelper,
        IExpHelper expHelper, IConfiguration configuration,IBizXmlRepository bizXmlRepository)
    {
        _bizXmlRepository = bizXmlRepository;
        _bizRepository = bizRepository;
        _jsonHelper = jsonHelper;
        _expHelper = expHelper;
    }

    
    // input data sample :      [{"bizid":{"val":"112"},"len":{"val":"10"},"wid":{"val":"20"},"lay":{"val":"2"},"qty":{"val":"5"},"thick":{"val":"1.6"},"surfish":{"val":"hasl"},"copthick":{"val":"1oz"},"mat":{"val":"fr4"},"solcol":{"val":"green"},"silcol":{"val":"white"},"type":{"val":"reg"},"__res__price":{"val":""},"__res__delivery":{"val":""},"files":{"val":"undefined"}}]        
    public async Task<string> GetCalculatedBizForm(string requestBody)
    {
        var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        
        // todo: make calculation from string exp and data then return the calculated values.
    
        Tokenize(requestJson);

        var biz = await _bizRepository.GetByIdAsync(BigInteger.Parse(dataPool["BizId"]));
        if (biz != null)
            _biz = biz;
        else
            throw new Exception("business not implemented yet!!!");


        PrepareExps(_biz);
        PrepareFuncs(_biz);
        PrepareLookup(_biz);
        
        execLookups();
        execFuncs();

        var result = _jsonHelper.ConvertKeyValuePairsToJson(_expHelper.MergeExpAndData(dataPool, expPool));
        return result;

        // return "Calc Function not implemented yet!!";
    }
    private void LookupHandle(string lookupStr)
    {
        var lookupDic = new Dictionary<string, string>();
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
            }
        }


        var objects = JArray.Parse(lookupStr); // parse as array
        foreach (JObject root in objects)
        {
            //Console.WriteLine(root);
            foreach (KeyValuePair<string, JToken> param in root)
            {
                if ((string)param.Value["exp"] != null)
                {
                    var strFormula = new string((string)param.Value["exp"]).Replace("\\\"", "'");
                    expression.SetFomular(strFormula);
                    try
                    {
                        lookupDic.Add(param.Key, expression.Eval().ToString());
                    }
                    catch
                    {
                    }

                    break;
                }
                else
                {
                    lookupDic.Add(param.Key, (string)param.Value["val"]);
                    break;
                }
            }
        }

        var xmlData = _bizXmlRepository.GetBizXmlAsDictionary(_biz, lookupDic);
        foreach (var (key, value) in xmlData.Where(kv => !dataPool.ContainsKey(kv.Key)))
            dataPool.Add(key,value);
    }

    /// <summary>
    /// input type: {{'BizId':{'val':'xxx'}},{'len':{'val':'xxx'}},{'wid':{'val':'xxx'}}}{{'BizId':{'val':'yyy'}},{'len':{'val':'xxx'}},{'wid':{'val':'xxx'}}}
    /// The method will call a biz by BizId and fill the values to get the return/s of the biz.
    /// the format of calling a function is same as a client call like: "{{'BizId':{'val':'xxx'}},{'len':{'val':'xxx'}},{'wid':{'val':'xxx'}}}"
    /// return type of the func will be pair of (key,value) which will add to a dictionary and return by the method.
    /// </summary>
    /// <param name="funcStr"></param>
    /// <returns>Dictionary<key:string, value:string></returns>
    public void FuncHandle(string funcStr)
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
            //Console.WriteLine(root);
            foreach (KeyValuePair<string, JToken> param in root)
            {
                if ((string)param.Value["exp"] != null)
                {
                    expression.SetFomular(new string((string)param.Value["exp"]).Replace("\\\"", "\""));

                    keyValuePairs.Add(param.Key, expression.Eval().ToString());
                    break;
                }
                else
                {
                    keyValuePairs.Add(param.Key, (string)param.Value["val"]);
                }
            }
        }

        Tokenize(GetCalculatedBizForm(_jsonHelper.ConvertKeyValuePairsToJson(keyValuePairs)).Result);
    }

    // sample input:
    // [{'__res__price':{'exp':'switch((if(wid <= 10 && len <= 10 && qty = 5, if(lay = 2, 1, if(lay = 4, 2, if(lay = 6, 4, 3))),3)),1, TEXT(CEILING((((((if(surfish = \"hasl\",0, goldfee)+if(solcol = \"black\", blackfee, if(solcol = \"green\", 0, colorfee)))*regProf))*rmbRate)+(Spec2Lay))/qty/100) *100*qty),2, TEXT(CEILING((((((if(surfish = \"hasl\",0, goldfee)+if(solcol = \"black\", blackfee, if(solcol = \"green\", 0, colorfee)))*regProf))*rmbRate)+(Spec4Lay))/qty/100) *100*qty),4, TEXT(CEILING((((((if(surfish = \"hasl\",0, goldfee)+if(solcol = \"black\", blackfee, if(solcol = \"green\", 0, colorfee)))*regProf))*rmbRate)+(Spec6Lay))/qty/100) *100*qty), 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = \"hasl\",brdrate, goldrate))+if(surfish = \"hasl\",0, goldfee)+projfee+if(solcol = \"black\", blackfee, if(solcol = \"green\", 0, colorfee)))*regProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*regShip))*rmbRate)/qty/100) *100*qty), \"not implemented Yet\")'}},{'__res__schprice':{'exp':'switch((if( wid <= 10 && len <= 10 && qty = 5 , if(lay = 2, 1, if(lay = 4, 2,  if(lay = 6, 4, 3))),3)),1, \"Not Supported\",2, \"Not Supported\",4, \"Not Supported\", 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = \"hasl\",brdrate, goldrate))+if(surfish = \"hasl\",0, goldfee)+projfee+if(solcol = \"black\", blackfee, if(solcol = \"green\", 0, colorfee)))*schProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*schShip))*rmbRate)/qty/100) *100*qty), \"not implemented Yet\")'}},{'__res__delivery':{'exp':'day+regShipday'}},{'__res__schdelivery':{'exp':'day+schShipday'}}]
    private void PrepareExps(Biz biz)
    {
        var objects = JArray.Parse(biz.Exp);
        foreach (JObject root in objects)
        {
            foreach (KeyValuePair<string, JToken> param in root)
            {
                expPool.Add(param.Key, (string)param.Value[EXP]);
            }
        }
    }
    // expPool on return:
    // "[__res__price, switch((if(wid <= 10 && len <= 10 && qty = 5, if(lay = 2, 1, if(lay = 4, 2, if(lay = 6, 4, 3))),3)),1, TEXT(CEILING((((((if(surfish = ""hasl"",0, goldfee)+if(solcol = ""black"", blackfee, if(solcol = ""green"", 0, colorfee)))*regProf))*rmbRate)+(Spec2Lay))/qty/100) *100*qty),2, TEXT(CEILING((((((if(surfish = ""hasl"",0, goldfee)+if(solcol = ""black"", blackfee, if(solcol = ""green"", 0, colorfee)))*regProf))*rmbRate)+(Spec4Lay))/qty/100) *100*qty),4, TEXT(CEILING((((((if(surfish = ""hasl"",0, goldfee)+if(solcol = ""black"", blackfee, if(solcol = ""green"", 0, colorfee)))*regProf))*rmbRate)+(Spec6Lay))/qty/100) *100*qty), 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = ""hasl"",brdrate, goldrate))+if(surfish = ""hasl"",0, goldfee)+projfee+if(solcol = ""black"", blackfee, if(solcol = ""green"", 0, colorfee)))*regProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*regShip))*rmbRate)/qty/100) *100*qty), ""not implemented Yet"")]"
    // "[__res__schprice, switch((if( wid <= 10 && len <= 10 && qty = 5 , if(lay = 2, 1, if(lay = 4, 2,  if(lay = 6, 4, 3))),3)),1, ""Not Supported"",2, ""Not Supported"",4, ""Not Supported"", 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = ""hasl"",brdrate, goldrate))+if(surfish = ""hasl"",0, goldfee)+projfee+if(solcol = ""black"", blackfee, if(solcol = ""green"", 0, colorfee)))*schProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*schShip))*rmbRate)/qty/100) *100*qty), ""not implemented Yet"")]"
    // "[__res__delivery, day+regShipday]"
    // "[__res__schdelivery, day+schShipday]"

    private void PrepareFuncs(Biz biz)
    {
        if (biz.Func.Equals(""))
            return;
        var objects = JArray.Parse(biz.Func); // parse as array
        foreach (JObject root in objects)
        {
            // Console.WriteLine(root);
            foreach (KeyValuePair<string, JToken> param in root)
            {
                funcPool.Add(param.Key, (string)param.Value[FUNC]);
            }
        }
    }

    // sample input:
    // [{'pcbrate':{'lookup':"[{'table':{'val':'jlcpcb2'}},{'layer':{'val':'xxx','exp':'lay'}},{'area':{'val':'20','exp':'len*wid*qty'}}]"}}, {'statics':{'lookup':"[{'table':{'val':'settings'}}]"}}]
    private void PrepareLookup(Biz biz)
    {
        if (_biz.Lookup.Equals(""))
            return;

        var objects = JArray.Parse(biz.Lookup); 
        foreach (JObject root in objects)
        {
            foreach (KeyValuePair<string, JToken> param in root)
            {
                lookupPool.Add(param.Key, (string)param.Value[LOOKUP]);
            }
        }
    }
    //lookupPool on return:
    // "[pcbrate, [{'table':{'val':'jlcpcb2'}},{'layer':{'val':'xxx','exp':'lay'}},{'area':{'val':'20','exp':'len*wid*qty'}}]]",pcbrate,"[{'table':{'val':'jlcpcb2'}},{'layer':{'val':'xxx','exp':'lay'}},{'area':{'val':'20','exp':'len*wid*qty'}}]"
    // "[statics, [{'table':{'val':'settings'}}]]",statics,[{'table':{'val':'settings'}}]

    private void execLookups()
    {
        foreach (var item in lookupPool)
        {
            LookupHandle(item.Value);
        }
    }

    private void execFuncs()
    {
        foreach (var item in funcPool)
        {
            FuncHandle(item.Value);
        }
    }

    private double? GetNumber(string v)
    {
        throw new NotImplementedException();
    }

    private bool loadBizDataFromDB_(string data)
    {
        //some functions to load data from DB
        /* only for test */
        if (true /*data exists in DB*/)
        {
            if (data.Equals("112"))
            {
                //ExpStr = "[{'price':{'exp':'((((len*wid*qty*brdfee)+colorfee)*1.8)+(weight*250))*9200'}},{'delivery':{'exp':'2+12'}},{'weight':{'exp':'weight'}},{'boardfee':{'exp':'brdfee'}}]";
                //functions = "[{'weight':{'func':\"[{'BizId':{'val':'111'}},{'area':{'val':'xxx','exp':'len*wid'}},{'wid':{'val':'xxx','exp':'wid'}},{'qty':{'val':'xxx','exp':'qty'}},{'thick':{'val':'xxx','exp':'thick'}}]\"}}]";
                /*,'exp':'if(qty<10, 5, if(qty<50, '10-30', '50up'))'*/

                //lookups = "[{'pcbrate,delivery':{'lookup':\"[{'table':{'val':'jlcpcb'}},{'lay':{'val':'xxx','exp':'lay'}},{'qty':{'val':'10-30','exp':'switch(if(qty<10, 1, if(qty<50, 2, 3)), 1, \\\"5\\\", 2 , \\\"10-30\\\", 3, \\\"50up\\\" , \\\"50up\\\")'}}]\"}}]";


                _biz.Lookup =
                    "[{'pcbrate':{'lookup':\"[{'table':{'val':'jlcpcb2'}},{'layer':{'val':'xxx','exp':'lay'}},{'area':{'val':'20','exp':'len*wid*qty'}}]\"}}, {'statics':{'lookup':\"[{'table':{'val':'settings'}}]\"}}]";

                //ExpStr = "[{'price':{'exp':'switch(5, 1,\\\"sdfsd\\\", 2, \\\"sdfsdf\\\", 5, TEXT(34563245), \\\"234234\\\")'}}]";

                _biz.Exp =
                    "[{'__res__price':{'exp':'switch((if(wid <= 10 && len <= 10 && qty = 5, if(lay = 2, 1, if(lay = 4, 2, if(lay = 6, 4, 3))),3)),1, TEXT(CEILING((((((if(surfish = \\\"hasl\\\",0, goldfee)+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf))*rmbRate)+(Spec2Lay))/qty/100) *100*qty),2, TEXT(CEILING((((((if(surfish = \\\"hasl\\\",0, goldfee)+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf))*rmbRate)+(Spec4Lay))/qty/100) *100*qty),4, TEXT(CEILING((((((if(surfish = \\\"hasl\\\",0, goldfee)+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf))*rmbRate)+(Spec6Lay))/qty/100) *100*qty), 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = \\\"hasl\\\",brdrate, goldrate))+if(surfish = \\\"hasl\\\",0, goldfee)+projfee+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*regShip))*rmbRate)/qty/100) *100*qty), \\\"not implemented Yet\\\")'}}" +
                    ",{'__res__schprice':{'exp':'switch((if( wid <= 10 && len <= 10 && qty = 5 , if(lay = 2, 1, if(lay = 4, 2,  if(lay = 6, 4, 3))),3)),1, \\\"Not Supported\\\",2, \\\"Not Supported\\\",4, \\\"Not Supported\\\", 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = \\\"hasl\\\",brdrate, goldrate))+if(surfish = \\\"hasl\\\",0, goldfee)+projfee+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*schProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*schShip))*rmbRate)/qty/100) *100*qty), \\\"not implemented Yet\\\")'}}" +
                    ",{'__res__delivery':{'exp':'day+regShipday'}}" +
                    ",{'__res__schdelivery':{'exp':'day+schShipday'}}]";
                //functions = "[{'weight':{'func':\"[{'BizId':{'val':'111'}},{'area':{'val':'xxx','exp':'len*wid'}},{'wid':{'val':'xxx','exp':'wid'}},{'qty':{'val':'xxx','exp':'qty'}},{'thick':{'val':'xxx','exp':'thick'}}]\"}}]";

                return true;
            }
            else if (data.Equals("111"))
            {
                _biz.Exp =
                    "[{'shipping':{'exp':'((area*qty*thick*0.00019*250))*9200'}},{'weight':{'exp':'(area*qty*thick*0.00019)'}},{'date':{'exp':'10'}}]";
                return true;
            }
            else if (data.Equals("110"))
            {
                _biz.Exp = "[{'price':{'exp':'11110000'}}]";
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private void Tokenize(string data)
    {
        var objects = JArray.Parse(data); // parse as array
        foreach (JObject root in objects)
        {
            //Console.WriteLine(root);
            foreach (KeyValuePair<string, JToken> param in root)
            {
                @dataPool.Add(param.Key, (string)param.Value["val"]);
            }
        }
    }
    // sample data pool after Tokenize
    // "[bizid, 112]",bizid,112
    // "[len, 10]",len,10
    // "[wid, 40]",wid,40
    // "[lay, 2]",lay,2
    // "[qty, 5]",qty,5
    // "[thick, 1.6]",thick,1.6
    // "[surfish, hasl]",surfish,hasl
    // "[copthick, 1oz]",copthick,1oz
    // "[mat, fr4]",mat,fr4
    // "[solcol, green]",solcol,green
    // "[silcol, white]",silcol,white
    // "[type, reg]",type,reg
    // "[__res__price, ]",__res__price,""
    // "[__res__delivery, ]",__res__delivery,""
    // "[files, undefined]",files,undefined

}
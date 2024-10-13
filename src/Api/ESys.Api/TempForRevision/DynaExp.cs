using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Newtonsoft.Json.Linq;
using Expression = org.matheval.Expression;

public class DynaExp
{
    private const string connectionString = @"Server=DESKTOP-OGLQH4M;Database=GlobalBizBuilder;User Id=sa;Password=1;Integrated Security=true;TrustServerCertificate=True;";

    private const string EXP = "exp";
    private const string LOOKUP = "lookup";
    private const string FUNC = "func";

    private Dictionary<string, string> dataPool = new Dictionary<string, string>();
    private Dictionary<string, string> expPool = new Dictionary<string, string>();
    private Dictionary<string, string> funcPool = new Dictionary<string, string>();
    private Dictionary<string, string> lookupPool = new Dictionary<string, string>();

    // these parameters should read from DB later
    private string ExpStr = "";
    private string lookups = "";
    private string functions = "";
    private string BizId = "112";
    private string xmlTable = "dbo.tblBizXmls";
    private string bizTable = "dbo.tblBiz";

    private bool LoadBizFromDB(string bizID)
    {

        SqlCommand command;
        SqlDataReader reader;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string queryStatement = "SELECT Exp, Func, lookup  FROM " + bizTable + " WHERE BizId = '" + bizID + "'";//queryBuilder(lookupStr);

            using (command = new SqlCommand(queryStatement, connection))
            {
                connection.Open();

                using (reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ExpStr = reader.GetString(0);
                        functions = reader.GetString(1);
                        lookups = reader.GetString(2);
                    }
                    if (ExpStr != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
            }
        }
    }

    private void RequestDBforXML(Dictionary<string, string> lookupStr)
    {
        SqlCommand command;
        SqlDataReader reader;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string queryStatement = "SELECT xmlTitles, xmlTags, xmlColCount, whereClause FROM " + xmlTable + " WHERE BizId = '" + BizId + "' and tname = '" + lookupStr["table"] + "'";//queryBuilder(lookupStr);
            string xmlTitles = "";
            string xmlTags = "";
            string xmlColCount = "1";
            string whereClause = "";
            using (command = new SqlCommand(queryStatement, connection))
            {
                connection.Open();

                using (reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        xmlTitles = reader.GetString(0);
                        xmlTags = reader.GetString(1);
                        xmlColCount = reader.GetString(2);
                        whereClause = reader.GetString(3);
                    }

                }
            }
            queryStatement = queryBuilder(lookupStr, xmlTitles, xmlTags, whereClause);
            using (command = new SqlCommand(queryStatement, connection))
            {

                using (reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (dataPool.ContainsKey(reader.GetName(i)))
                            {
                                continue;
                            }
                            if (reader.GetFieldType(i) == typeof(Int32))
                                dataPool.Add(reader.GetName(i), reader.GetInt32(i).ToString());
                            else
                                dataPool.Add(reader.GetName(i), reader.GetString(i));
                        }
                    }

                }
            }

        }
    }

    private void rextest()
    {

    }

    private string queryBuilder(Dictionary<string, string> lookupStr, string xmlTitles, string xmlTags, string whereClause)
    {

        string query = @"SELECT * FROM ( SELECT" +
                xmlTitles + " FROM(select * FROM " +
                xmlTable +
                " WHERE BizId = '" +
                BizId +
                "' AND tname = '" +
                lookupStr["table"] +
                "') e OUTER APPLY e.xml.nodes('" +
                xmlTags +
                "') as X(Y) )T ";

        lookupStr.Remove("table");

        if (lookupStr.Count() > 0)
        {
            if (whereClause != null)
            {
                string tmpWhere = whereClause;
                foreach (var item in lookupStr)
                {
                    tmpWhere = tmpWhere.Replace("__" + item.Key, item.Value);
                }
                query += tmpWhere;
            }


        }

        /* {
             query += " WHERE ";

             bool isFirst = true;
             foreach (var item in lookupStr)
             {
                 if (isFirst)
                 {
                     query += item.Key + " = '" + item.Value + "' ";
                     isFirst = false;
                 }
                 else
                 {
                     query += " AND " + item.Key + " = '" + item.Value + "' ";
                 }

             }

         }*/


        return query;
    }

    private void LookupHandle(string lookupStr)
    {



        //return;
        var pairs = new Dictionary<string, string>();
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


        var objects = JArray.Parse(lookupStr); // parse as array
        foreach (JObject root in objects)
        {
            //Console.WriteLine(root);
            foreach (KeyValuePair<string, JToken> param in root)
            {
                if ((string)param.Value["exp"] != null)
                {
                    // if (qty < 10, 5, if (qty < 50, "10-30", "50up"))
                    /*Expression ex = new Expression("SWITCH(qty, <10, '5', <50, '10 - 30', '50up')");
                    ex.Bind("TAX_CODE", "GST");
                    ex.Bind("amount", 5005m);
                    String value = ex.Eval<String>();*/

                    var strFormula = new string((string)param.Value["exp"]).Replace("\\\"", "'");
                    expression.SetFomular(strFormula);
                    try
                    {
                        pairs.Add(param.Key, expression.Eval().ToString());
                    }
                    catch
                    {

                    }
                    break;
                }
                else
                {
                    pairs.Add(param.Key, (string)param.Value["val"]);
                    break;
                }

            }
        }
        RequestDBforXML(pairs);
        // catchXMLData(pairs);

    }

    private void catchXMLData(Dictionary<string, string> lookMap)
    {



        XmlDocument doc = new XmlDocument(); /* using System.Xml */
        doc.Load("../../db/xmls/" + lookMap["table"]);
        DataSet dataSet = new DataSet();
        dataSet.ReadXml(new StringReader(doc.InnerXml));

        //Console.WriteLine(dataSet.Tables[0]);

        Console.WriteLine(dataSet.Tables[0].Select());
        /* foreach (DataRow dataRow in dataSet.Tables[0].Rows)
         {
             foreach (var item in dataRow.ItemArray)
             {
                 Console.WriteLine(item);
             }
         }*/

        //return dataSet.Tables[0];


        /* XElement root = XElement.Load("../../db/xmls/" + lookMap["table"]);
         IEnumerable<XElement> row =
             from el in root.Elements()
             where (string)el.Elements().Elements("lay") == "2"
             select el;
         foreach (XElement el in row)
             Console.WriteLine(el);*/

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
        var pairs = new Dictionary<string, string>();


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

                    pairs.Add(param.Key, expression.Eval().ToString());
                    break;
                }
                else
                {
                    pairs.Add(param.Key, (string)param.Value["val"]);
                }

            }
        }
        execJson(jsonBuild(pairs));
    }

    private string jsonBuild(Dictionary<string, string> pairs)
    {
        string json = "[{";

        var first = true;

        foreach (var item in pairs)
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

        json = json + "}]";



        return json;
    }


    private void execJson(string json)
    {
        Tokenize(new DynaExp().DynaCalcByExp(json));
    }

    private void prepair(string expStr, string type)
    {
        var pairs = expPool;
        if (type.Equals(EXP))
        {
            pairs = expPool;
        }
        else if (type.Equals(FUNC))
        {
            if (functions.Equals(""))
            {
                return;
            }
            pairs = funcPool;
        }
        else if (type.Equals(LOOKUP))
        {
            if (lookups.Equals(""))
            {
                return;
            }
            pairs = lookupPool;
        }
        else
        {
            return;
        }

        var objects = JArray.Parse(expStr); // parse as array
        foreach (JObject root in objects)
        {
            // Console.WriteLine(root);
            foreach (KeyValuePair<string, JToken> param in root)
            {

                pairs.Add(param.Key, (string)param.Value[type]);
            }
        }

    }
    private string expSolver(string exp)
    {
        String str = new String(exp).Replace("\\\"", "\"");

        Expression expression = new Expression(str);
        foreach (var item in dataPool)
        {
            try
            {
                expression.Bind(item.Key, Convert.ToDouble(item.Value));
            }
            catch (Exception)
            {

                //throw;
            }

        }
        Object value = expression.Eval();

        return value.ToString();
    }

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
    private string execExps()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

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

        foreach (var exp in expPool)
        {
            expression.SetFomular(exp.Value);
            Object value = expression.Eval();
            result.Add(exp.Key, value.ToString());
        }

        return jsonBuild(result);

    }


    public DynaExp()
    {
    }
    public DynaExp(string expID)
    {
        //*** todo: query to the DB for getting the exp by expID from DB and load to the Exp
        // Exp = receiveExpByID(expID);
        Exp = @"len,*,wid,*,qty,*,qty";

    }


    public string Exp { get; set; }
    public string DynaCalcByExp(string data)
    {
        //*** todo: make calculation from string exp and data then return the calculated values.

        Tokenize(data);

        /*var expArr = Exp.Split(',');
        double result = 0;*/

        //check database for request of ui Json (surveyjs)
        if (dataPool.ContainsKey("BizJson"))
        {

            return "{\r\n\r\n    \"title\": \"PCB From Class 1\",\r\n    \"completedHtml\": \"<h3>Thank you for your feedback</h3>\",\r\n    \"completedHtmlOnCondition\": [\r\n        {\r\n            \"html\": \"<h3>Thank you for your feedback</h3> <h4>We are glad that you love our product. Your ideas and suggestions will help us make it even better.</h4>\"\r\n        },\r\n        {\r\n            \"html\": \"<h3>Thank you for your feedback</h3> <h4>We are glad that you shared your ideas with us. They will help us make our product better.</h4>\"\r\n        }\r\n    ],\r\n    \"pages\": [\r\n        {\r\n            \"name\": \"page1\",\r\n            \"elements\": [\r\n                {\r\n                    \"type\": \"panel\",\r\n                    \"name\": \"panel1\",\r\n                    \"elements\": [\r\n                        {\r\n                            \"type\": \"text\",\r\n                            \"name\": \"len\",\r\n                            \"width\": \"5%\",\r\n                            \"minWidth\": \"100px\",\r\n                            \"title\": \"طول\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"10\",\r\n                            \"errorLocation\": \"bottom\"\r\n                        },\r\n                        {\r\n                            \"type\": \"text\",\r\n                            \"name\": \"wid\",\r\n                            \"width\": \"5%\",\r\n                            \"minWidth\": \"100px\",\r\n                            \"title\": \"عرض\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"10\",\r\n                            \"errorLocation\": \"bottom\",\r\n                            \"startWithNewLine\": false\r\n                        },\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"lay\",\r\n                            \"width\": \"10%\",\r\n                            \"minWidth\": \"150px\",\r\n                            \"title\": \"تعداد لایه\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"2\",\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"2\",\r\n                                    \"text\": \"2 لایه\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"4\",\r\n                                    \"text\": \"4 لایه\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"6\",\r\n                                    \"text\": \"6 لایه\"\r\n                                }\r\n                            ],\r\n                            \"allowClear\": false,\r\n                            \"startWithNewLine\": false\r\n                        },\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"qty\",\r\n                            \"width\": \"5%\",\r\n                            \"minWidth\": \"120px\",\r\n                            \"title\": \"تعداد\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"5\",\r\n                            \"choices\": [\r\n                                \"5\",\r\n                                \"10\",\r\n                                \"15\",\r\n                                \"20\",\r\n                                \"25\",\r\n                                \"30\",\r\n                                \"50\"\r\n                            ],\r\n                            \"allowClear\": false,\r\n                            \"startWithNewLine\": false\r\n                        },\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"thick\",\r\n                            \"width\": \"6%\",\r\n                            \"minWidth\": \"130px\",\r\n                            \"title\": \"ضخامت برد\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"setValueExpression\": \"iif({layers} = 4 and {thick} < 1.0 , 1.0 ,iif({layers} = 6 and {thick} < 1.2, 1.2 , {thick}))\",\r\n                            \"defaultValue\": \"1.6\",\r\n                            \"choices\": [\r\n                                \"0.6\",\r\n                                \"0.8\",\r\n                                \"1.0\",\r\n                                \"1.2\",\r\n                                \"1.6\",\r\n                                \"2.0\"\r\n                            ],\r\n                            \"allowClear\": false,\r\n                            \"startWithNewLine\": false\r\n                        },\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"surfish\",\r\n                            \"width\": \"8%\",\r\n                            \"minWidth\": \"150px\",\r\n                            \"title\": \"پوشش نهایی\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"hasl\",\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"hasl\",\r\n                                    \"text\": \"HASL\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"enig\",\r\n                                    \"text\": \"ENIG\"\r\n                                }\r\n                            ],\r\n                            \"allowClear\": false,\r\n                            \"startWithNewLine\": false\r\n                        },\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"copthick\",\r\n                            \"width\": \"11%\",\r\n                            \"minWidth\": \"220px\",\r\n                            \"title\": \"ضخامت مس\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"1oz\",\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"1oz\",\r\n                                    \"text\": \"1OZ (35 میکرون)\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"2oz\",\r\n                                    \"text\": \"2OZ (70 میکرون)\"\r\n                                }\r\n                            ],\r\n                            \"allowClear\": false,\r\n                            \"startWithNewLine\": false\r\n                        },\r\n\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"mat\",\r\n                            \"width\": \"8%\",\r\n                            \"minWidth\": \"240px\",\r\n                            \"title\": \"جنس برد\",\r\n                            \"titleLocation\": \"top\",\r\n                            \"defaultValue\": \"fr4\",\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"fr4\",\r\n                                    \"text\": \"FR4-TG135\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"alu\",\r\n                                    \"text\": \"Aluminum\"\r\n                                }\r\n                            ],\r\n                            \"allowClear\": false,\r\n                            \"startWithNewLine\": false\r\n                        },\r\n                        {\r\n                            \"type\": \"radiogroup\",\r\n                            \"name\": \"solcol\",\r\n                            \"title\": \"Solder Mask Color\",\r\n                            \"setValueExpression\": \"iif({silcol}='black' ,  'white' , iif({solcol}='white', green, {solcol}))\",\r\n                            \"defaultValue\": \"green\",\r\n                            \"isRequired\": true,\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"green\",\r\n                                    \"text\": \"Green\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"blue\",\r\n                                    \"text\": \"Blue\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"red\",\r\n                                    \"text\": \"Red\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"yellow\",\r\n                                    \"text\": \"Yellow\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"black\",\r\n                                    \"text\": \"Black\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"white\",\r\n                                    \"text\": \"White\"\r\n                                }\r\n                            ],\r\n                            \"colCount\": 4\r\n                        },\r\n                        {\r\n                            \"type\": \"radiogroup\",\r\n                            \"name\": \"silcol\",\r\n                            \"startWithNewLine\": false,\r\n                            \"title\": \"Silkscreen Color\",\r\n                            \"setValueExpression\": \"iif({solcol}='white' ,  'black' , 'white')\",\r\n                            \"defaultValue\": \"white\",\r\n                            \"defaultValueExpression\": \"iif({solcol}='white' ,  'black' , 'white')\",\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"white\",\r\n                                    \"text\": \"White\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"black\",\r\n                                    \"text\": \"Black\"\r\n                                }\r\n                            ]\r\n                        },\r\n                        {\r\n                            \"type\": \"dropdown\",\r\n                            \"name\": \"type\",\r\n                            \"startWithNewLine\": false,\r\n                            \"title\": \"Production Plan\",\r\n                            \"defaultValue\": \"reg\",\r\n                            \"choices\": [\r\n                                {\r\n                                    \"value\": \"reg\",\r\n                                    \"text\": \"Regular\"\r\n                                },\r\n                                {\r\n                                    \"value\": \"sch\",\r\n                                    \"text\": \"Scheduled\"\r\n                                }\r\n                            ],\r\n                            \"allowClear\": false\r\n                        }\r\n                    ],\r\n                    \"maxWidth\": \"70%\"\r\n                },\r\n                {\r\n                    \"type\": \"panel\",\r\n                    \"name\": \"panel2\",\r\n                    \"elements\": [\r\n                        {\r\n                            \"type\": \"text\",\r\n                            \"defaultValue\": \"-\",\r\n                            \"name\": \"__res__price\",\r\n                            \"title\": \"Price\",\r\n                            \"readOnly\": true\r\n                        },\r\n                        {\r\n                            \"type\": \"text\",\r\n                            \"defaultValue\": \"-\",\r\n                            \"name\": \"__res__delivery\",\r\n                            \"title\": \"Delivery\",\r\n                            \"readOnly\": true\r\n                        }\r\n                    ],\r\n                    \"startWithNewLine\": false,\r\n                    \"maxWidth\": \"30%\"\r\n                },\r\n                {\r\n                    \"type\": \"file\",\r\n                    \"name\": \"files\",\r\n                    \"title\": \"Upload File\",\r\n                    \"storeDataAsText\": false,\r\n                    \"allowMultiple\": true,\r\n                    \"maxSize\": 102400,\r\n                    \"address\": \"https://api.surveyjs.io/public/v1/Survey/upload/\"\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"showQuestionNumbers\": \"off\"\r\n    \r\n}";
        }

        if (!loadBizDataFromDB(dataPool["BizId"]))
        {
            return "business not implemented yet!!!";
        }


        prepair(ExpStr, EXP);
        prepair(functions, FUNC);
        prepair(lookups, LOOKUP);

        execLookups();
        execFuncs();

        return execExps();
        //return expTest(dataMap, ExpStr);

        // return "Calc Function not implemented yet!!";
    }


    private double? GetNumber(string v)
    {
        throw new NotImplementedException();
    }

    private bool loadBizDataFromDB(string data)
    {
        //some functions to load data from DB
        /* only for test */
        if (LoadBizFromDB(data))
        {
            return true;
        }

        return false;
    }

    private bool loadBizDataFromDB_(string data)
    {
        //some functions to load data from DB
        /* only for test */
        if (true /*data exists in DB*/ )
        {
            if (data.Equals("112"))
            {
                //ExpStr = "[{'price':{'exp':'((((len*wid*qty*brdfee)+colorfee)*1.8)+(weight*250))*9200'}},{'delivery':{'exp':'2+12'}},{'weight':{'exp':'weight'}},{'boardfee':{'exp':'brdfee'}}]";
                //functions = "[{'weight':{'func':\"[{'BizId':{'val':'111'}},{'area':{'val':'xxx','exp':'len*wid'}},{'wid':{'val':'xxx','exp':'wid'}},{'qty':{'val':'xxx','exp':'qty'}},{'thick':{'val':'xxx','exp':'thick'}}]\"}}]";
                /*,'exp':'if(qty<10, 5, if(qty<50, '10-30', '50up'))'*/

                //lookups = "[{'pcbrate,delivery':{'lookup':\"[{'table':{'val':'jlcpcb'}},{'lay':{'val':'xxx','exp':'lay'}},{'qty':{'val':'10-30','exp':'switch(if(qty<10, 1, if(qty<50, 2, 3)), 1, \\\"5\\\", 2 , \\\"10-30\\\", 3, \\\"50up\\\" , \\\"50up\\\")'}}]\"}}]";


                lookups = "[{'pcbrate':{'lookup':\"[{'table':{'val':'jlcpcb2'}},{'layer':{'val':'xxx','exp':'lay'}},{'area':{'val':'20','exp':'len*wid*qty'}}]\"}}, {'statics':{'lookup':\"[{'table':{'val':'settings'}}]\"}}]";

                //ExpStr = "[{'price':{'exp':'switch(5, 1,\\\"sdfsd\\\", 2, \\\"sdfsdf\\\", 5, TEXT(34563245), \\\"234234\\\")'}}]";

                ExpStr = "[{'__res__price':{'exp':'switch((if(wid <= 10 && len <= 10 && qty = 5, if(lay = 2, 1, if(lay = 4, 2, if(lay = 6, 4, 3))),3)),1, TEXT(CEILING((((((if(surfish = \\\"hasl\\\",0, goldfee)+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf))*rmbRate)+(Spec2Lay))/qty/100) *100*qty),2, TEXT(CEILING((((((if(surfish = \\\"hasl\\\",0, goldfee)+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf))*rmbRate)+(Spec4Lay))/qty/100) *100*qty),4, TEXT(CEILING((((((if(surfish = \\\"hasl\\\",0, goldfee)+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf))*rmbRate)+(Spec6Lay))/qty/100) *100*qty), 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = \\\"hasl\\\",brdrate, goldrate))+if(surfish = \\\"hasl\\\",0, goldfee)+projfee+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*regProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*regShip))*rmbRate)/qty/100) *100*qty), \\\"not implemented Yet\\\")'}}" +
                         ",{'__res__schprice':{'exp':'switch((if( wid <= 10 && len <= 10 && qty = 5 , if(lay = 2, 1, if(lay = 4, 2,  if(lay = 6, 4, 3))),3)),1, \\\"Not Supported\\\",2, \\\"Not Supported\\\",4, \\\"Not Supported\\\", 3, TEXT(CEILING(MAX(if(lay = 2, Spec2Lay, if(lay = 4, Spec4Lay, Spec6Lay)) ,((((len*wid*qty*if(surfish = \\\"hasl\\\",brdrate, goldrate))+if(surfish = \\\"hasl\\\",0, goldfee)+projfee+if(solcol = \\\"black\\\", blackfee, if(solcol = \\\"green\\\", 0, colorfee)))*schProf)+(ROUND(len*thick*wid*qty*0.000208125,2)*schShip))*rmbRate)/qty/100) *100*qty), \\\"not implemented Yet\\\")'}}" +
                         ",{'__res__delivery':{'exp':'day+regShipday'}}" +
                         ",{'__res__schdelivery':{'exp':'day+schShipday'}}]";
                //functions = "[{'weight':{'func':\"[{'BizId':{'val':'111'}},{'area':{'val':'xxx','exp':'len*wid'}},{'wid':{'val':'xxx','exp':'wid'}},{'qty':{'val':'xxx','exp':'qty'}},{'thick':{'val':'xxx','exp':'thick'}}]\"}}]";

                return true;
            }
            else if (data.Equals("111"))
            {
                ExpStr = "[{'shipping':{'exp':'((area*qty*thick*0.00019*250))*9200'}},{'weight':{'exp':'(area*qty*thick*0.00019)'}},{'date':{'exp':'10'}}]";
                return true;
            }
            else if (data.Equals("110"))
            {
                ExpStr = "[{'price':{'exp':'11110000'}}]";
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
    
}

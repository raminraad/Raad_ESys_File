namespace ESys.Persistence.Static;
/// <summary>
/// Static values containing table names in SQL Server. Mostly used by Dapper
/// </summary>
public static class SqlServerStatics
{
    public static string BizTable => "dbo.tblBiz";
    public static string XmlTable => "dbo.tblBizXmls";
    public static string BizInitialUiTable => "dbo.tblUI";
}

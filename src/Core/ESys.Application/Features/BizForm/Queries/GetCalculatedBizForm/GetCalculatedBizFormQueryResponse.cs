namespace ESys.Application.Features.BizForm.Queries.GetCalculatedBizForm;

/// <summary>
/// Response being sent back to client containing calculated data for BizForm
/// </summary>
public class GetCalculatedBizFormQueryResponse
{
    /// <summary>
    /// Contains a Json string filled with calculated data for BizForm
    /// </summary>
    public string Result { get; set; } 
}
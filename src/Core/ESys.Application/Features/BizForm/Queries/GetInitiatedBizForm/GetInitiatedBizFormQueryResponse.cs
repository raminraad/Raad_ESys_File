namespace ESys.Application.Features.BizForm.Queries.GetInitiatedBizForm;

/// <summary>
/// Response being sent back to client containing initialization data for BizForm
/// </summary>
public class GetInitiatedBizFormQueryResponse
{
    /// <summary>
    /// Contains a Json string filled with needed data for BizForm initialization
    /// </summary>
    public string Result { get; set; } = string.Empty;
}
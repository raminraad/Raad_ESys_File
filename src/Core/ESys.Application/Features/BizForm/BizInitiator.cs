using ESys.Application.Contracts.Persistence;

namespace ESys.Application.Features.BizForm;

public class BizInitiator
{
    private readonly IBizInitialUiRepository _bizInitialUiRepository;

    public BizInitiator(IBizInitialUiRepository bizInitialUiRepository)
    {
        _bizInitialUiRepository = bizInitialUiRepository;
    }

    public async Task<string> GetInitialBizForm(string bizId)
    {
        var bizUi = await _bizInitialUiRepository.GetByIdAsync(bizId);
        //todo check for nulls
        return bizUi.UiJson;
    }
}
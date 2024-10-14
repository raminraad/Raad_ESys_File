using ESys.Application.Contracts.Persistence;
using ESys.Application.Exceptions;
using ESys.Domain.Entities;

namespace ESys.Application.Features.BizForm;

public class BizFormInitiator
{
    private readonly IBizInitialUiRepository _bizInitialUiRepository;

    public BizFormInitiator(IBizInitialUiRepository bizInitialUiRepository)
    {
        _bizInitialUiRepository = bizInitialUiRepository;
    }

    /// <summary>
    /// Gets BizForm initial UI from database and returns it for initial rendering
    /// </summary>
    /// <param name="bizId">BizId which corresponding UI must be returned</param>
    /// <returns>A Json string containing all the data needed for BizForm initialization</returns>
    /// <exception cref="NotFoundException">Occurs when there is no UI for received BizId in database</exception>
    public async Task<string> GetInitialBizForm(string bizId)
    {
            var bizUi = await _bizInitialUiRepository.GetByIdAsync(bizId);
            if (bizUi is null)
                throw new NotFoundException(nameof(BizInitialUi), bizId);
            return bizUi.UiJson;
    }
}
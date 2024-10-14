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

    public async Task<string> GetInitialBizForm(string bizId)
    {
            var bizUi = await _bizInitialUiRepository.GetByIdAsync(bizId);
            if (bizUi is null)
                throw new NotFoundException(nameof(BizInitialUi), bizId);
            return bizUi.UiJson;
    }
}
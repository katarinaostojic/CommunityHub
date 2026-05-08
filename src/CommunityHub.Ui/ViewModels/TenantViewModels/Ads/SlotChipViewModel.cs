using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class SlotChipViewModel : BaseViewModel
{
    private readonly AdSlot _slot;

    public SlotChipViewModel(AdSlot slot)
    {
        _slot = slot;
    }

    public string Display => $"{_slot.StartTime:HH:mm} - {_slot.EndTime:HH:mm}";
}
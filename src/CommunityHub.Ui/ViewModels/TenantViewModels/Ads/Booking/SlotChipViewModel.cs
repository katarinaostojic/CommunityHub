using CommunityHub.Application.DTOs.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads.Booking;

public class SlotChipViewModel : BaseViewModel
{
    private readonly AdSlotDto _slot;

    public SlotChipViewModel(AdSlotDto slot)
    {
        _slot = slot;
    }

    public string Display => _slot.TimeDisplay;
}
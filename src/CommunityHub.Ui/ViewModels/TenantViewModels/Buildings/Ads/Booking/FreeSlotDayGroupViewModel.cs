using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.Booking;

public class FreeSlotDayGroupViewModel : BaseViewModel
{
    public FreeSlotDayGroupViewModel(DateOnly date, List<AdSlotDto> slots)
    {
        DateDisplay = date.ToString("dd.MM");
        Slots = slots.Select(s => new SlotChipViewModel(s)).ToList();
    }

    public string DateDisplay { get; }
    public List<SlotChipViewModel> Slots { get; }
}
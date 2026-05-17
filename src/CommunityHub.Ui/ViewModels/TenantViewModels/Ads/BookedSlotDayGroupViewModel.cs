using CommunityHub.Application.DTOs.TenantAds;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class BookedSlotDayGroupViewModel : BaseViewModel
{
    public BookedSlotDayGroupViewModel(DateOnly date, List<AdSlotDto> slots)
    {
        DateDisplay = date.ToString("dd.MM.");
        Slots = slots.Select(s => new SlotChipViewModel(s)).ToList();
    }

    public string DateDisplay { get; }
    public List<SlotChipViewModel> Slots { get; }
}
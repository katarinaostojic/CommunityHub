using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.Booking;

public class SelectableFreeSlotDayGroupViewModel : BaseViewModel
{
    public SelectableFreeSlotDayGroupViewModel(DateOnly date, List<AdSlotDto> slots)
    {
        DateDisplay = date.ToString("dddd, dd.MM.").ToUpper();
        Slots = slots.Select(s => new SelectableSlotChipViewModel(s)).ToList();
    }

    public string DateDisplay { get; }
    public List<SelectableSlotChipViewModel> Slots { get; }
}
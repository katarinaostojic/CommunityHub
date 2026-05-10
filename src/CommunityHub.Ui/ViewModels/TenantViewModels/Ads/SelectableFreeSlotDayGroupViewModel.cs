using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class SelectableFreeSlotDayGroupViewModel : BaseViewModel
{
    public SelectableFreeSlotDayGroupViewModel(DateOnly date, List<AdSlot> slots)
    {
        DateDisplay = date.ToString("dddd, dd.MM.").ToUpper();
        Slots = slots.Select(s => new SelectableSlotChipViewModel(s)).ToList();
    }

    public string DateDisplay { get; }
    public List<SelectableSlotChipViewModel> Slots { get; }
}
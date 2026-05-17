using CommunityHub.Application.DTOs.TenantAds;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class SelectableSlotChipViewModel : BaseViewModel
{
    private bool _isSelected;

    public SelectableSlotChipViewModel(AdSlotDto slot)
    {
        Slot = slot;
        Display = slot.TimeDisplay;
    }

    public AdSlotDto Slot { get; }
    public string Display { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public void ToggleSelection() => IsSelected = !IsSelected;
}
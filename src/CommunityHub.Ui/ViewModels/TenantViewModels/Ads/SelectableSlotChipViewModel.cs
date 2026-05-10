using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class SelectableSlotChipViewModel : BaseViewModel
{
    private bool _isSelected;

    public SelectableSlotChipViewModel(AdSlot slot)
    {
        Slot = slot;
        Display = $"{slot.StartTime:HH:mm} - {slot.EndTime:HH:mm}";
    }

    public AdSlot Slot { get; }
    public string Display { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public void ToggleSelection() => IsSelected = !IsSelected;
}
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Services.Interfaces.Ads;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads.Booking;

public class BookSlotsViewModel : BaseViewModel
{
    private readonly IAdSlotBookingService _slotBookingService;
    private readonly AdDto _theirAd;
    private readonly AdDto _myAd;
    private string _selectedCountText = string.Empty;
    private bool _hasSelectedSlots;
    private bool _hasNoSlots;

    public BookSlotsViewModel(
        IAdSlotBookingService slotBookingService,
        AdDto theirAd,
        AdDto myAd)
    {
        _slotBookingService = slotBookingService;
        _theirAd = theirAd;
        _myAd = myAd;

        AuthorName = theirAd.AuthorName;
        TypeDisplay = theirAd.Type.ToDisplayString();
        CategoryDisplay = theirAd.Category.ToDisplayString();
        DateRangeDisplay = $"{theirAd.DateFrom:dd.MM.yyyy} – {theirAd.DateTo:dd.MM.yyyy}";

        (DateOnly slotFrom, DateOnly slotTo) = theirAd.GetOverlapWith(myAd);
        OverlapRangeDisplay = $"Showing slots within your overlap: {slotFrom:dd.MM.} – {slotTo:dd.MM.} only";

        List<AdSlotDto> freeSlots = _slotBookingService.GetFreeSlots(theirAd.Id, slotFrom, slotTo);
        HasNoSlots = freeSlots.Count == 0;

        FreeSlotDayGroups = new ObservableCollection<SelectableFreeSlotDayGroupViewModel>(
            freeSlots
                .GroupBy(s => s.Date)
                .OrderBy(g => g.Key)
                .Select(g => new SelectableFreeSlotDayGroupViewModel(g.Key, g.ToList())));

        UpdateSelection();
    }

    public string AuthorName { get; }
    public string TypeDisplay { get; }
    public string CategoryDisplay { get; }
    public string DateRangeDisplay { get; }
    public string OverlapRangeDisplay { get; }

    public ObservableCollection<SelectableFreeSlotDayGroupViewModel> FreeSlotDayGroups { get; }

    public string SelectedCountText
    {
        get => _selectedCountText;
        private set => SetProperty(ref _selectedCountText, value);
    }

    public bool HasSelectedSlots
    {
        get => _hasSelectedSlots;
        private set => SetProperty(ref _hasSelectedSlots, value);
    }

    public bool HasNoSlots
    {
        get => _hasNoSlots;
        private set => SetProperty(ref _hasNoSlots, value);
    }

    public void ToggleSlot(SelectableSlotChipViewModel slot)
    {
        slot.ToggleSelection();
        UpdateSelection();
    }

    public bool BookSelectedSlots()
    {
        List<long> selectedIds = FreeSlotDayGroups
            .SelectMany(g => g.Slots)
            .Where(s => s.IsSelected)
            .Select(s => s.Slot.Id)
            .ToList();

        if (selectedIds.Count == 0) return false;

        _slotBookingService.BookSlots(selectedIds, _myAd.Id, _theirAd.Id);
        return true;
    }

    private void UpdateSelection()
    {
        int count = FreeSlotDayGroups
            .SelectMany(g => g.Slots)
            .Count(s => s.IsSelected);

        HasSelectedSlots = count > 0;
        SelectedCountText = count == 0
            ? "No slots selected"
            : $"{count} slot{(count == 1 ? "" : "s")} selected";
    }
}
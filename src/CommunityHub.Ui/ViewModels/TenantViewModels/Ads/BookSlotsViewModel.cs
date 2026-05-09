using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class BookSlotsViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly Ad _myAd;
    private string _selectedCountText = string.Empty;
    private bool _hasSelectedSlots;
    private bool _hasNoSlots;

    public BookSlotsViewModel(AdService adService, Ad theirAd, Ad myAd)
    {
        _adService = adService;
        _myAd = myAd;

        AuthorName = theirAd.Author.DisplayName;
        TypeDisplay = theirAd.Type.ToDisplayString();
        CategoryDisplay = theirAd.Category.ToDisplayString();
        DateRangeDisplay = $"{theirAd.DateFrom:dd.MM.yyyy} – {theirAd.DateTo:dd.MM.yyyy}";

        DateOnly overlapFrom = theirAd.DateFrom > myAd.DateFrom ? theirAd.DateFrom : myAd.DateFrom;
        DateOnly overlapTo = theirAd.DateTo < myAd.DateTo ? theirAd.DateTo : myAd.DateTo;

        OverlapRangeDisplay = $"Showing slots within your overlap: {overlapFrom:dd.MM.} – {overlapTo:dd.MM.} only";

        List<AdSlot> freeSlots = adService.GetFreeSlots(theirAd.Id, overlapFrom, overlapTo);
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

        if (selectedIds.Count == 0)
            return false;

        _adService.BookSlots(selectedIds, _myAd.Id);
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
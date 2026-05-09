using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class BookSlotsViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly Ad _theirAd;
    private readonly Ad _myAd;
    private string _confirmButtonText = string.Empty;
    private bool _hasNoSlots;

    public BookSlotsViewModel(AdService adService, Ad theirAd, Ad myAd)
    {
        _adService = adService;
        _theirAd = theirAd;
        _myAd = myAd;

        AuthorName = theirAd.Author.DisplayName;
        TypeDisplay = theirAd.Type.ToDisplayString();
        CategoryDisplay = theirAd.Category.ToDisplayString();
        DateRangeDisplay = $"{theirAd.DateFrom:dd.MM.yyyy} – {theirAd.DateTo:dd.MM.yyyy}";
        Description = theirAd.Description;

        DateOnly overlapFrom = theirAd.DateFrom > myAd.DateFrom ? theirAd.DateFrom : myAd.DateFrom;
        DateOnly overlapTo = theirAd.DateTo < myAd.DateTo ? theirAd.DateTo : myAd.DateTo;

        List<AdSlot> freeSlots = adService.GetFreeSlots(theirAd.Id, overlapFrom, overlapTo);
        HasNoSlots = freeSlots.Count == 0;

        FreeSlotDayGroups = new ObservableCollection<SelectableFreeSlotDayGroupViewModel>(
            freeSlots
                .GroupBy(s => s.Date)
                .OrderBy(g => g.Key)
                .Select(g => new SelectableFreeSlotDayGroupViewModel(g.Key, g.ToList())));

        UpdateConfirmButton();
    }

    public string AuthorName { get; }
    public string TypeDisplay { get; }
    public string CategoryDisplay { get; }
    public string DateRangeDisplay { get; }
    public string Description { get; }

    public ObservableCollection<SelectableFreeSlotDayGroupViewModel> FreeSlotDayGroups { get; }

    public string ConfirmButtonText
    {
        get => _confirmButtonText;
        private set => SetProperty(ref _confirmButtonText, value);
    }

    public bool HasNoSlots
    {
        get => _hasNoSlots;
        private set => SetProperty(ref _hasNoSlots, value);
    }

    public void ToggleSlot(SelectableSlotChipViewModel slot)
    {
        slot.ToggleSelection();
        UpdateConfirmButton();
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

    private void UpdateConfirmButton()
    {
        int count = FreeSlotDayGroups
            .SelectMany(g => g.Slots)
            .Count(s => s.IsSelected);

        ConfirmButtonText = count == 0
            ? "Select at least one slot"
            : $"Book {count} slot{(count == 1 ? "" : "s")}";
    }
}
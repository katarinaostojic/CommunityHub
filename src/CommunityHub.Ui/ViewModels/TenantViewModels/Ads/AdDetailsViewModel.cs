using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class AdDetailsViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _adId;

    private ObservableCollection<BookedSlotGroupViewModel> _bookedSlotGroups = new();
    private ObservableCollection<FreeSlotDayGroupViewModel> _freeSlotDayGroups = new();
    private string _bookedSlotsTitleText = string.Empty;

    public AdDetailsViewModel(Ad ad, AdService adService)
    {
        _adService = adService;
        _adId = ad.Id;

        TypeDisplay = ad.Type.ToDisplayString();
        CategoryDisplay = ad.Category.ToDisplayString();
        DateRangeDisplay = $"{ad.DateFrom:dd.MM.} - {ad.DateTo:dd.MM.yyyy}";
        Description = ad.Description;
        IsActive = ad.IsActive;

        LoadBookedSlots();
        LoadFreeSlots(ad.DateFrom, ad.DateTo);
    }

    public string TypeDisplay { get; }
    public string CategoryDisplay { get; }
    public string DateRangeDisplay { get; }
    public string Description { get; }
    public bool IsActive { get; }

    public Visibility ArchivedWarningVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;
    public Visibility ArchiveButtonVisible => IsActive ? Visibility.Visible : Visibility.Collapsed;
    public Visibility RestoreButtonVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;
    public Visibility AdStatusTextVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;

    public string BookedSlotsTitleText
    {
        get => _bookedSlotsTitleText;
        private set => SetProperty(ref _bookedSlotsTitleText, value);
    }

    public ObservableCollection<BookedSlotGroupViewModel> BookedSlotGroups
    {
        get => _bookedSlotGroups;
        private set => SetProperty(ref _bookedSlotGroups, value);
    }

    public ObservableCollection<FreeSlotDayGroupViewModel> FreeSlotDayGroups
    {
        get => _freeSlotDayGroups;
        private set => SetProperty(ref _freeSlotDayGroups, value);
    }

    public void ArchiveAd() => _adService.Archive(_adId);

    public void RestoreAd() => _adService.Restore(_adId);

    public Ad? GetRefreshedAd() => _adService.GetById(_adId);

    private void LoadBookedSlots()
    {
        List<(AdSlot slot, Ad? bookedByAd)> bookedWithAds = _adService.GetBookedSlotsWithAds(_adId);

        List<BookedSlotGroupViewModel> grouped = bookedWithAds
            .GroupBy(x => x.bookedByAd?.Id)
            .Select(g => new BookedSlotGroupViewModel(
                g.Select(x => x.slot).ToList(),
                g.First().bookedByAd))
            .ToList();

        BookedSlotGroups = new ObservableCollection<BookedSlotGroupViewModel>(grouped);
        BookedSlotsTitleText = $"Booked slots ({bookedWithAds.Count})";
    }

    private void LoadFreeSlots(DateOnly dateFrom, DateOnly dateTo)
    {
        List<FreeSlotDayGroupViewModel> grouped = _adService
            .GetFreeSlots(_adId, dateFrom, dateTo)
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new FreeSlotDayGroupViewModel(g.Key, g.ToList()))
            .ToList();

        FreeSlotDayGroups = new ObservableCollection<FreeSlotDayGroupViewModel>(grouped);
    }
}
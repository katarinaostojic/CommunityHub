using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Services.Interfaces.Ads;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads.Booking;

public class AdSlotsViewModel : BaseViewModel
{
    private readonly IAdSlotBookingService _slotBookingService;
    private readonly long _adId;
    private readonly DateOnly _dateFrom;
    private readonly DateOnly _dateTo;

    private ObservableCollection<BookedSlotGroupViewModel> _bookedSlotGroups = new();
    private ObservableCollection<FreeSlotDayGroupViewModel> _freeSlotDayGroups = new();
    private string _bookedSlotsTitleText = string.Empty;

    public AdSlotsViewModel(
        IAdSlotBookingService slotBookingService,
        long adId,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        _slotBookingService = slotBookingService;
        _adId = adId;
        _dateFrom = dateFrom;
        _dateTo = dateTo;
    }

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

    public void LoadSlots()
    {
        LoadBookedSlots();
        LoadFreeSlots();
    }

    private void LoadBookedSlots()
    {
        List<BookedAdSlotDto> bookedWithAds = _slotBookingService.GetBookedSlotsWithAds(_adId);

        List<BookedSlotGroupViewModel> grouped = bookedWithAds
            .GroupBy(x => x.BookedByAd?.Id)
            .Select(g => new BookedSlotGroupViewModel(
                g.Select(x => x.Slot).ToList(),
                g.First().BookedByAd))
            .ToList();

        BookedSlotGroups = new ObservableCollection<BookedSlotGroupViewModel>(grouped);
        BookedSlotsTitleText = $"Booked slots ({bookedWithAds.Count})";
    }

    private void LoadFreeSlots()
    {
        List<FreeSlotDayGroupViewModel> grouped = _slotBookingService
            .GetFreeSlots(_adId, _dateFrom, _dateTo)
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new FreeSlotDayGroupViewModel(g.Key, g.ToList()))
            .ToList();

        FreeSlotDayGroups = new ObservableCollection<FreeSlotDayGroupViewModel>(grouped);
    }
}
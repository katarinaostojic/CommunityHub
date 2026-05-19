using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;

namespace CommunityHub.Application.Services.Ads;

public class AdSlotBookingService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotRepository _adSlotRepository;
    private readonly IAdNotificationRepository _notificationRepository;

    private static readonly TimeOnly SlotStart = new TimeOnly(16, 0);
    private const int SlotsPerDay = 4;

    public AdSlotBookingService(
        IAdRepository adRepository,
        IAdSlotRepository adSlotRepository,
        IAdNotificationRepository notificationRepository)
    {
        _adRepository = adRepository;
        _adSlotRepository = adSlotRepository;
        _notificationRepository = notificationRepository;
    }

    public void CreateSlotsForAd(long adId, DateOnly dateFrom, DateOnly dateTo)
    {
        _adSlotRepository.CreateSlots(adId, GenerateSlots(dateFrom, dateTo));
    }

    public List<AdSlotDto> GetFreeSlots(
        long adId,
        DateOnly overlapFrom,
        DateOnly overlapTo)
    {
        return _adSlotRepository
            .GetFreeSlotsByAd(adId, overlapFrom, overlapTo)
            .ToAdSlotDtoList();
    }

    public List<BookedAdSlotDto> GetBookedSlotsWithAds(long adId)
    {
        return _adSlotRepository
            .GetBookedSlotsWithAds(adId)
            .ToBookedAdSlotDtoList();
    }

    public List<AdSlotDto> GetBookedSlots(long adId)
    {
        return _adSlotRepository
            .GetBookedSlotsByAd(adId)
            .ToAdSlotDtoList();
    }

    public void BookSlots(IEnumerable<long> slotIds, long bookedByAdId, long ownerAdId)
    {
        foreach (long slotId in slotIds)
            _adSlotRepository.BookSlot(slotId, bookedByAdId);

        Ad? ownerAd = _adRepository.GetById(ownerAdId);
        if (ownerAd == null) return;

        _notificationRepository.Create(ownerAd.Author.Id, ownerAdId, bookedByAdId);
    }

    private List<(DateOnly, TimeOnly, TimeOnly)> GenerateSlots(DateOnly dateFrom, DateOnly dateTo)
    {
        List<(DateOnly, TimeOnly, TimeOnly)> slots = new();

        for (DateOnly date = dateFrom; date <= dateTo; date = date.AddDays(1))
            AddDailySlots(slots, date);

        return slots;
    }

    private static void AddDailySlots(
        List<(DateOnly, TimeOnly, TimeOnly)> slots,
        DateOnly date)
    {
        for (int i = 0; i < SlotsPerDay; i++)
        {
            TimeOnly startTime = SlotStart.AddHours(i);
            TimeOnly endTime = SlotStart.AddHours(i + 1);
            slots.Add((date, startTime, endTime));
        }
    }
}
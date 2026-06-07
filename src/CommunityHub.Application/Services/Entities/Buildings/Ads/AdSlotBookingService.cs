using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.Ads;
using CommunityHub.Application.DtoMappers.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Application.Services.Entities.Buildings.Ads;

public class AdSlotBookingService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotRepository _adSlotRepository;
    private readonly IAdNotificationRepository _notificationRepository;

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
        _adSlotRepository.CreateSlots(adId, AdSlot.GenerateForDateRange(dateFrom, dateTo));
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

    public void BookSlots(IEnumerable<long> slotIds, long bookedByAdId, long ownerAdId)
    {
        foreach (long slotId in slotIds)
            _adSlotRepository.BookSlot(slotId, bookedByAdId);

        Ad? ownerAd = _adRepository.GetById(ownerAdId);
        if (ownerAd == null) return;

        _notificationRepository.CreateBookingNotification(ownerAd.Author.Id, ownerAdId, bookedByAdId);
    }
}
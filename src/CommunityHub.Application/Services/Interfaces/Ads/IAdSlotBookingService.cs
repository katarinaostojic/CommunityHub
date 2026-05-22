using CommunityHub.Application.DTOs.Ads;

namespace CommunityHub.Application.Services.Interfaces.Ads;

public interface IAdSlotBookingService
{
    void CreateSlotsForAd(long adId, DateOnly dateFrom, DateOnly dateTo);

    List<AdSlotDto> GetFreeSlots(
        long adId,
        DateOnly overlapFrom,
        DateOnly overlapTo);

    List<BookedAdSlotDto> GetBookedSlotsWithAds(long adId);

    void BookSlots(IEnumerable<long> slotIds, long bookedByAdId, long ownerAdId);
}
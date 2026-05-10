namespace CommunityHub.Application.Domain.Ads;

public interface IAdSlotRepository
{
    void CreateSlots(long adId, IEnumerable<(DateOnly date, TimeOnly start, TimeOnly end)> slots);
    List<AdSlot> GetSlotsByAd(long adId);
    List<AdSlot> GetFreeSlotsByAd(long adId, DateOnly overlapFrom, DateOnly overlapTo);
    void BookSlot(long slotId, long bookedByAdId);
    List<AdSlot> GetBookedSlotsByAd(long adId);
    List<(AdSlot slot, Ad? bookedByAd)> GetBookedSlotsWithAds(long adId);
}
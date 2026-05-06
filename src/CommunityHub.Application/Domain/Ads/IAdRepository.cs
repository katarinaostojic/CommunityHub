namespace CommunityHub.Application.Domain.Ads;

public interface IAdRepository
{
    List<Ad> GetActiveByBuilding(long buildingId);
    Ad? GetById(long adId);
    long Create(long buildingId, long authorId, AdType type, AdCategory category,
        string description, DateOnly dateFrom, DateOnly dateTo);
    void Archive(long adId);
    void Restore(long adId);
    void CreateSlots(long adId, IEnumerable<(DateOnly date, TimeOnly start, TimeOnly end)> slots);
    List<AdSlot> GetSlotsByAd(long adId);
    List<AdSlot> GetFreeSlotsByAd(long adId, DateOnly overlapFrom, DateOnly overlapTo);
    void BookSlot(long slotId, long bookedByAdId);
    List<AdSlot> GetBookedSlotsByAd(long adId);
}
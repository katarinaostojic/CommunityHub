using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Application.Services.Ads;

public class AdService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotRepository _adSlotRepository;

    private static readonly TimeOnly SlotStart = new TimeOnly(16, 0);
    private const int SlotsPerDay = 4;

    public AdService(IAdRepository adRepository, IAdSlotRepository adSlotRepository)
    {
        _adRepository = adRepository;
        _adSlotRepository = adSlotRepository;
    }

    public List<Ad> GetActiveByBuilding(long buildingId)
    {
        return _adRepository.GetActiveByBuilding(buildingId);
    }

    public Ad? GetById(long adId)
    {
        return _adRepository.GetById(adId);
    }

    public Ad Create(long buildingId, long authorId, AdType type, AdCategory category,
        string description, DateOnly dateFrom, DateOnly dateTo)
    {
        long adId = _adRepository.Create(buildingId, authorId, type, category,
            description, dateFrom, dateTo);

        _adSlotRepository.CreateSlots(adId, GenerateSlots(dateFrom, dateTo));

        return _adRepository.GetById(adId)!;
    }

    public void Archive(long adId)
    {
        Ad ad = _adRepository.GetById(adId)!;
        ad.Archive();
        _adRepository.Update(ad);
    }

    public void Restore(long adId)
    {
        Ad ad = _adRepository.GetById(adId)!;
        ad.Restore();
        _adRepository.Update(ad);
    }

    public List<AdSlot> GetFreeSlots(long adId, DateOnly overlapFrom, DateOnly overlapTo)
    {
        return _adSlotRepository.GetFreeSlotsByAd(adId, overlapFrom, overlapTo);
    }

    public List<(AdSlot slot, Ad? bookedByAd)> GetBookedSlotsWithAds(long adId)
    {
        return _adSlotRepository.GetBookedSlotsWithAds(adId);
    }

    public List<AdSlot> GetBookedSlots(long adId)
    {
        return _adSlotRepository.GetBookedSlotsByAd(adId);
    }

    public void BookSlots(IEnumerable<long> slotIds, long bookedByAdId)
    {
        foreach (long slotId in slotIds)
            _adSlotRepository.BookSlot(slotId, bookedByAdId);
    }

    public List<Ad> FindMatchingAds(Ad newAd)
    {
        List<Ad> activeAds = _adRepository.GetActiveByBuilding(newAd.BuildingId);

        AdType oppositeType = newAd.Type == AdType.Offering
            ? AdType.Seeking
            : AdType.Offering;

        return activeAds
            .Where(ad => ad.Id != newAd.Id
                && ad.Author.Id != newAd.Author.Id
                && ad.Type == oppositeType
                && ad.Category == newAd.Category
                && ad.OverlapsWith(newAd.DateFrom, newAd.DateTo))
            .ToList();
    }

    private List<(DateOnly, TimeOnly, TimeOnly)> GenerateSlots(DateOnly dateFrom, DateOnly dateTo)
    {
        List<(DateOnly, TimeOnly, TimeOnly)> slots = new List<(DateOnly, TimeOnly, TimeOnly)>();

        for (DateOnly date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            for (int i = 0; i < SlotsPerDay; i++)
            {
                TimeOnly startTime = SlotStart.AddHours(i);
                TimeOnly endTime = SlotStart.AddHours(i + 1);
                slots.Add((date, startTime, endTime));
            }
        }

        return slots;
    }
}
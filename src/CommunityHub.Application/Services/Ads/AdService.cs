using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Ads.AdRepositoryInterfaces;

namespace CommunityHub.Application.Services.Ads;

public class AdService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotRepository _adSlotRepository;
    private readonly IAdNotificationRepository _notificationRepository;

    private static readonly TimeOnly SlotStart = new TimeOnly(16, 0);
    private const int SlotsPerDay = 4;

    public AdService(IAdRepository adRepository, IAdSlotRepository adSlotRepository,
        IAdNotificationRepository notificationRepository)
    {
        _adRepository = adRepository;
        _adSlotRepository = adSlotRepository;
        _notificationRepository = notificationRepository;
    }

    public List<Ad> GetActiveByBuilding(long buildingId)
    {
        return _adRepository.GetActiveByBuilding(buildingId);
    }

    public Ad? GetById(long adId)
    {
        return _adRepository.GetById(adId);
    }

    public Ad Create(Ad ad)
    {
        long adId = _adRepository.Create(ad);
        _adSlotRepository.CreateSlots(adId, GenerateSlots(ad.DateFrom, ad.DateTo));
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

    public void BookSlots(IEnumerable<long> slotIds, long bookedByAdId, long ownerAdId)
    {
        foreach (long slotId in slotIds)
            _adSlotRepository.BookSlot(slotId, bookedByAdId);

        Ad? ownerAd = _adRepository.GetById(ownerAdId);
        if (ownerAd == null) return;

        _notificationRepository.Create(ownerAd.Author.Id, ownerAdId, bookedByAdId);
    }

    public List<AdNotification> GetUnreadNotifications(long userId)
    {
        return _notificationRepository.GetUnreadByUser(userId);
    }

    public void MarkNotificationAsRead(long notificationId)
    {
        _notificationRepository.MarkAsRead(notificationId);
    }

    public void MarkAllNotificationsAsRead(long userId)
    {
        _notificationRepository.MarkAllAsRead(userId);
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

    public List<Ad> GetAllByBuilding(long buildingId)
    {
        return _adRepository.GetAllByBuilding(buildingId);
    }

    public int CountByType(List<Ad> ads, AdType type)
    {
        return ads.Count(a => a.Type == type);
    }

    public Dictionary<AdCategory, (int offering, int seeking)> GetStatsByCategory(List<Ad> ads)
    {
        return ads
            .GroupBy(a => a.Category)
            .ToDictionary(
                g => g.Key,
                g => (
                    offering: g.Count(a => a.Type == AdType.Offering),
                    seeking: g.Count(a => a.Type == AdType.Seeking)
                )
            );
    }

    public (int active, int archived) GetCurrentState(List<Ad> ads)
    {
        return (
            active: ads.Count(a => a.Status == AdStatus.Active),
            archived: ads.Count(a => a.Status == AdStatus.Archived)
        );
    }

    public Dictionary<AdCategory, int> GetActiveCountByCategory(List<Ad> ads)
    {
        return ads
            .Where(a => a.Status == AdStatus.Active)
            .GroupBy(a => a.Category)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public User? GetTopHelper(long buildingId)
    {
        return _adSlotRepository.GetTopHelperByBuilding(buildingId);
    }
}
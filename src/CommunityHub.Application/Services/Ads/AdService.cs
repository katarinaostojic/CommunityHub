using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Ads.AdRepositoryInterfaces;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;

namespace CommunityHub.Application.Services.Ads;

public class AdService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotRepository _adSlotRepository;
    private readonly IAdNotificationRepository _notificationRepository;

    private static readonly TimeOnly SlotStart = new TimeOnly(16, 0);
    private const int SlotsPerDay = 4;

    public AdService(
        IAdRepository adRepository,
        IAdSlotRepository adSlotRepository,
        IAdNotificationRepository notificationRepository)
    {
        _adRepository = adRepository;
        _adSlotRepository = adSlotRepository;
        _notificationRepository = notificationRepository;
    }

    public List<AdDto> GetActiveByBuilding(long buildingId)
    {
        RefreshExpiredAds(buildingId);
        return _adRepository.GetActiveByBuilding(buildingId).ToAdDtoList();
    }

    public List<AdDto> GetFilteredActiveByBuilding(
    long buildingId,
    AdType? type,
    AdCategory? category)
    {
        RefreshExpiredAds(buildingId);

        return _adRepository
            .GetFilteredActiveByBuilding(buildingId, type, category)
            .ToAdDtoList();
    }

    public int CountFilteredActiveByBuilding(
        long buildingId,
        AdType? type,
        AdCategory? category)
    {
        RefreshExpiredAds(buildingId);

        return _adRepository.CountFilteredActiveByBuilding(buildingId, type, category);
    }

    public AdDto? GetCurrentUserMatchingAd(long buildingId, long currentUserId, AdDto theirAd)
    {
        AdType myType = theirAd.Type == AdType.Offering
            ? AdType.Seeking
            : AdType.Offering;

        return GetFilteredActiveByBuilding(buildingId, myType, theirAd.Category)
            .FirstOrDefault(ad =>
                ad.AuthorId == currentUserId
                && ad.OverlapsWith(theirAd.DateFrom, theirAd.DateTo));
    }

    public AdDto? GetById(long adId)
    {
        return _adRepository.GetById(adId)?.ToAdDto();
    }

    public (AdDto newAd, List<AdDto> matchingAds) Create(
        long buildingId,
        User author,
        AdType type,
        AdCategory category,
        string description,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        Ad ad = new Ad(buildingId, author, type, category, description, dateFrom, dateTo);
        Ad newAd = CreateAdWithSlots(ad);
        List<AdDto> matchingAds = FindMatchingAds(newAd).ToAdDtoList();

        return (newAd.ToAdDto(), matchingAds);
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

    public List<AdNotificationDto> GetUnreadNotifications(long userId)
    {
        return _notificationRepository
            .GetUnreadByUser(userId)
            .ToAdNotificationDtoList();
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

    public void BookSlots(IEnumerable<long> slotIds, long bookedByAdId, long ownerAdId)
    {
        foreach (long slotId in slotIds)
            _adSlotRepository.BookSlot(slotId, bookedByAdId);

        Ad? ownerAd = _adRepository.GetById(ownerAdId);
        if (ownerAd == null) return;

        _notificationRepository.Create(ownerAd.Author.Id, ownerAdId, bookedByAdId);
    }

    public void MarkNotificationAsRead(long notificationId)
    {
        _notificationRepository.MarkAsRead(notificationId);
    }

    public void MarkAllNotificationsAsRead(long userId)
    {
        _notificationRepository.MarkAllAsRead(userId);
    }

    private void RefreshExpiredAds(long buildingId)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        List<Ad> expiredAds = _adRepository
            .GetActiveByBuilding(buildingId)
            .Where(ad => ad.IsExpired(today))
            .ToList();

        foreach (Ad ad in expiredAds)
        {
            ad.Archive();
            _adRepository.Update(ad);
        }
    }

    private Ad CreateAdWithSlots(Ad ad)
    {
        long adId = _adRepository.Create(ad);
        _adSlotRepository.CreateSlots(adId, GenerateSlots(ad.DateFrom, ad.DateTo));
        return _adRepository.GetById(adId)!;
    }

    private List<Ad> FindMatchingAds(Ad newAd)
    {
        RefreshExpiredAds(newAd.BuildingId);

        AdType oppositeType = newAd.Type == AdType.Offering
            ? AdType.Seeking
            : AdType.Offering;

        return _adRepository
            .GetFilteredActiveByBuilding(newAd.BuildingId, oppositeType, newAd.Category)
            .Where(ad => IsMatchingAd(ad, newAd))
            .ToList();
    }

    private static bool IsMatchingAd(Ad ad, Ad newAd)
    {
        return ad.Id != newAd.Id
            && ad.Author.Id != newAd.Author.Id
            && ad.OverlapsWith(newAd.DateFrom, newAd.DateTo);
    }

    private List<(DateOnly, TimeOnly, TimeOnly)> GenerateSlots(DateOnly dateFrom, DateOnly dateTo)
    {
        List<(DateOnly, TimeOnly, TimeOnly)> slots = new();

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

    // TODO: menadzer metode treba prebaciti da koriste DTO

    public List<AdDto> GetAllByBuilding(long buildingId)
    {
        RefreshExpiredAds(buildingId);
        return _adRepository.GetAllByBuilding(buildingId).ToAdDtoList();
    }

    public int CountByType(List<AdDto> ads, AdType type)
    {
        return ads.Count(a => a.Type == type);
    }

    public Dictionary<AdCategory, (int offering, int seeking)> GetStatsByCategory(List<AdDto> ads)
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

    public (int active, int archived) GetCurrentState(List<AdDto> ads)
    {
        return (
            active: ads.Count(a => a.Status == AdStatus.Active),
            archived: ads.Count(a => a.Status == AdStatus.Archived)
        );
    }

    public Dictionary<AdCategory, int> GetActiveCountByCategory(List<AdDto> ads)
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
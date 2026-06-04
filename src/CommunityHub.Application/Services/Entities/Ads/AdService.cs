using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;
using CommunityHub.Application.Services.Interfaces.Ads;

namespace CommunityHub.Application.Services.Entities.Ads;

public class AdService : IAdService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotBookingService _slotBookingService;
    private readonly AdExpirationService _expirationService;
    private readonly IAdNotificationRepository _notificationRepository;

    public AdService(
        IAdRepository adRepository,
        IAdNotificationRepository notificationRepository,
        IAdSlotBookingService slotBookingService,
        AdExpirationService expirationService)
    {
        _adRepository = adRepository;
        _notificationRepository = notificationRepository;
        _slotBookingService = slotBookingService;
        _expirationService = expirationService;
    }

    public List<AdDto> GetFilteredActiveByBuilding(
        long buildingId,
        AdType? type,
        AdCategory? category)
    {
        _expirationService.RefreshExpiredAds(buildingId);

        return _adRepository
            .GetFilteredActiveByBuilding(buildingId, type, category)
            .ToAdDtoList();
    }

    public int CountFilteredActiveByBuilding(
        long buildingId,
        AdType? type,
        AdCategory? category)
    {
        _expirationService.RefreshExpiredAds(buildingId);
        return _adRepository.CountFilteredActiveByBuilding(buildingId, type, category);
    }

    public AdDto? GetCurrentUserMatchingAd(long buildingId, long currentUserId, AdDto theirAd)
    {
        return GetFilteredActiveByBuilding(buildingId, theirAd.OppositeType, theirAd.Category)
            .FirstOrDefault(ad =>
                ad.AuthorId == currentUserId
                && ad.OverlapsWith(theirAd.DateFrom, theirAd.DateTo));
    }

    public AdDto? GetById(long adId)
    {
        return _adRepository.GetById(adId)?.ToAdDto();
    }

    public List<AdDto> GetReportAds(
        long buildingId,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        _expirationService.RefreshExpiredAds(buildingId);

        return _adRepository
            .GetAllByBuilding(buildingId)
            .Where(ad => ad.DateFrom <= dateTo && ad.DateTo >= dateFrom)
            .OrderBy(ad => ad.DateFrom)
            .ThenBy(ad => ad.DateTo)
            .ThenBy(ad => ad.Type)
            .ToAdDtoList();
    }

    public (AdDto newAd, List<AdDto> matchingAds) Create(CreateAdDto request)
    {
        Ad ad = new(
            request.BuildingId,
            request.Author,
            request.Type,
            request.Category,
            request.Description,
            request.DateFrom,
            request.DateTo);

        Ad newAd = CreateAdWithSlots(ad);
        List<Ad> matchingAds = FindMatchingAds(newAd);
        NotifyAdsWaitingForMatch(newAd, matchingAds);

        return (newAd.ToAdDto(), matchingAds.ToAdDtoList());
    }

    public void Archive(long adId)
    {
        Ad ad = GetRequiredAd(adId);

        ad.Archive();
        _adRepository.Update(ad);
    }

    public void Restore(long adId)
    {
        Ad ad = GetRequiredAd(adId);

        ad.Restore();
        _adRepository.Update(ad);
    }

    private Ad CreateAdWithSlots(Ad ad)
    {
        long adId = _adRepository.Create(ad);

        _slotBookingService.CreateSlotsForAd(adId, ad.DateFrom, ad.DateTo);

        return GetRequiredAd(adId);
    }

    private Ad GetRequiredAd(long adId)
    {
        return _adRepository.GetById(adId)
            ?? throw new InvalidOperationException("Ad was not found.");
    }

    private List<Ad> FindMatchingAds(Ad newAd)
    {
        _expirationService.RefreshExpiredAds(newAd.BuildingId);

        return _adRepository
            .GetFilteredActiveByBuilding(newAd.BuildingId, newAd.OppositeType, newAd.Category)
            .Where(ad => ad.IsEligibleMatchFor(newAd))
            .ToList();
    }

    private void NotifyAdsWaitingForMatch(Ad newAd, List<Ad> matchingAds)
    {
        foreach (Ad matchingAd in matchingAds.Where(IsWaitingForMatch))
        {
            _notificationRepository.CreateMatchingAdNotification(matchingAd.Author.Id, matchingAd.Id, newAd.Id);
        }

        bool IsWaitingForMatch(Ad matchingAd)
        {
            return !_adRepository.HasActiveMatchBefore(matchingAd, newAd.Id);
        }
    }
}
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;

namespace CommunityHub.Application.Services.Ads;

public class AdService
{
    private readonly IAdRepository _adRepository;
    private readonly AdSlotBookingService _slotBookingService;

    public AdService(
        IAdRepository adRepository,
        AdSlotBookingService slotBookingService)
    {
        _adRepository = adRepository;
        _slotBookingService = slotBookingService;
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
        AdType myType = GetOppositeType(theirAd.Type);

        return GetFilteredActiveByBuilding(buildingId, myType, theirAd.Category)
            .FirstOrDefault(ad =>
                ad.AuthorId == currentUserId
                && ad.OverlapsWith(theirAd.DateFrom, theirAd.DateTo));
    }

    public AdDto? GetById(long adId)
    {
        return _adRepository.GetById(adId)?.ToAdDto();
    }

    public (AdDto newAd, List<AdDto> matchingAds) Create(CreateAdDto request)
    {
        Ad ad = new Ad(
            request.BuildingId,
            request.Author,
            request.Type,
            request.Category,
            request.Description,
            request.DateFrom,
            request.DateTo);

        Ad newAd = CreateAdWithSlots(ad);
        List<AdDto> matchingAds = FindMatchingAds(newAd).ToAdDtoList();

        return (newAd.ToAdDto(), matchingAds);
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

    private Ad CreateAdWithSlots(Ad ad)
    {
        long adId = _adRepository.Create(ad);
        _slotBookingService.CreateSlotsForAd(adId, ad.DateFrom, ad.DateTo);
        return _adRepository.GetById(adId)!;
    }

    private List<Ad> FindMatchingAds(Ad newAd)
    {
        RefreshExpiredAds(newAd.BuildingId);

        AdType oppositeType = GetOppositeType(newAd.Type);

        return _adRepository
            .GetFilteredActiveByBuilding(newAd.BuildingId, oppositeType, newAd.Category)
            .Where(ad => IsEligibleMatch(ad, newAd))
            .ToList();
    }

    private static AdType GetOppositeType(AdType type)
    {
        return type == AdType.Offering
            ? AdType.Seeking
            : AdType.Offering;
    }

    private static bool IsEligibleMatch(Ad ad, Ad newAd)
    {
        return ad.Author.Id != newAd.Author.Id
            && ad.OverlapsWith(newAd.DateFrom, newAd.DateTo);
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
}
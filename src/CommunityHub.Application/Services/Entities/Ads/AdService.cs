using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;
using CommunityHub.Application.Services.Interfaces.Ads;

namespace CommunityHub.Application.Services.Entities.Ads;

public class AdService : IAdService
{
    private readonly IAdRepository _adRepository;
    private readonly AdSlotBookingService _slotBookingService;
    private readonly AdExpirationService _expirationService;

    public AdService(
        IAdRepository adRepository,
        AdSlotBookingService slotBookingService,
        AdExpirationService expirationService)
    {
        _adRepository = adRepository;
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
        _expirationService.RefreshExpiredAds(newAd.BuildingId);

        return _adRepository
            .GetFilteredActiveByBuilding(newAd.BuildingId, newAd.OppositeType, newAd.Category)
            .Where(ad => ad.IsEligibleMatchFor(newAd))
            .ToList();
    }
}
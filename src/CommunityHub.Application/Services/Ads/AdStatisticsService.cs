using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Ads.AdRepositoryInterfaces;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;

namespace CommunityHub.Application.Services.Ads;

public class AdStatisticsService
{
    private readonly IAdRepository _adRepository;
    private readonly IAdSlotRepository _adSlotRepository;

    public AdStatisticsService(
        IAdRepository adRepository,
        IAdSlotRepository adSlotRepository)
    {
        _adRepository = adRepository;
        _adSlotRepository = adSlotRepository;
    }

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
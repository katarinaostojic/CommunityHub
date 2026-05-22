using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;

namespace CommunityHub.Application.Services.Entities.Ads;

public class AdExpirationService
{
    private readonly IAdRepository _adRepository;

    public AdExpirationService(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public void RefreshExpiredAds(long buildingId)
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
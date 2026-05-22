using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.DTOs.Ads;

namespace CommunityHub.Application.Services.Interfaces.Ads;

public interface IAdService
{
    List<AdDto> GetFilteredActiveByBuilding(
        long buildingId,
        AdType? type,
        AdCategory? category);

    int CountFilteredActiveByBuilding(
        long buildingId,
        AdType? type,
        AdCategory? category);

    AdDto? GetCurrentUserMatchingAd(long buildingId, long currentUserId, AdDto theirAd);

    AdDto? GetById(long adId);

    List<AdDto> GetReportAds(
        long buildingId,
        DateOnly dateFrom,
        DateOnly dateTo);

    (AdDto newAd, List<AdDto> matchingAds) Create(CreateAdDto request);

    void Archive(long adId);

    void Restore(long adId);
}
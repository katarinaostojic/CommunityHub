using CommunityHub.Application.Domain.Entities.Ads;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Ads;

public interface IAdRepository
{
    List<Ad> GetActiveByBuilding(long buildingId);
    List<Ad> GetFilteredActiveByBuilding(long buildingId, AdType? type, AdCategory? category);
    int CountFilteredActiveByBuilding(long buildingId, AdType? type, AdCategory? category);
    Ad? GetById(long adId);
    long Create(Ad ad);
    void Update(Ad ad);
    List<Ad> GetAllByBuilding(long buildingId);
}
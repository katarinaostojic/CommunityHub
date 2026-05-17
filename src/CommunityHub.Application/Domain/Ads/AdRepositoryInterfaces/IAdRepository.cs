namespace CommunityHub.Application.Domain.Ads;

public interface IAdRepository
{
    List<Ad> GetActiveByBuilding(long buildingId);
    Ad? GetById(long adId);
    long Create(Ad ad);
    void Update(Ad ad);
    List<Ad> GetAllByBuilding(long buildingId);
}
using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface ICityGiftRepository
{
    List<CityGift> GetAll();
    CityGift? GetById(long id);
    List<CityGiftApplication> GetApplications(long cityGiftId);
    void Apply(long cityGiftId, long coordinatorId, long neighborhoodId);
    void Award(long cityGiftId, long neighborhoodId);
    void AddDonationToCategories(long neighborhoodId, decimal amountPerCategory);
    List<(long NeighborhoodId, decimal Budget)> GetBudgetsForNeighborhoods(List<long> neighborhoodIds);
}
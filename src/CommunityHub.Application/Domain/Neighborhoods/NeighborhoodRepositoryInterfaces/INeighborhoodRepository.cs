using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

public interface INeighborhoodRepository
{
    List<Neighborhood> SearchForCitizen(string? name, string? address, string? city, string? country);
    List<Neighborhood> GetByCoordinator(long coordinatorId);
    long Create(string name, string description, long cityId, long coordinatorId);
    void AddStreet(long neighborhoodId, string streetName, int startNumber, int endNumber);
    void AddImage(long neighborhoodId, string imagePath);
    Neighborhood? GetById(long id);
    string? GetNameById(long id);
}
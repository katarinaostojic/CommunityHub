using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Neighborhoods;

public class NeighborhoodService
{
    private readonly INeighborhoodRepository _repository;

    public NeighborhoodService(INeighborhoodRepository repository)
    {
        _repository = repository;
    }

    public List<Neighborhood> SearchForCitizen(string? name, string? address, string? city, string? country)
    {
        return _repository.SearchForCitizen(name, address, city, country);
    }

    public List<NeighborhoodDto> GetByCoordinator(long coordinatorId)
    {
        return _repository.GetByCoordinator(coordinatorId).ToDtoList();
    }
    public long Create(string name, string description, long cityId, long coordinatorId)
    {
        return _repository.Create(name, description, cityId, coordinatorId);
    }

    public void AddStreet(long neighborhoodId, string streetName, int startNumber, int endNumber)
    {
        _repository.AddStreet(neighborhoodId, streetName, startNumber, endNumber);
    }

    public void AddImage(long neighborhoodId, string imagePath)
    {
        _repository.AddImage(neighborhoodId, imagePath);
    }
}
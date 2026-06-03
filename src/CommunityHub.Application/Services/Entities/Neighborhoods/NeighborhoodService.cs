using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

public class NeighborhoodService
{
    private readonly INeighborhoodRepository _repository;

    public NeighborhoodService(INeighborhoodRepository repository)
    {
        _repository = repository;
    }

    public List<NeighborhoodDto> SearchForCitizen(string? name, string? address, string? city, string? country)
        => _repository.SearchForCitizen(name, address, city, country).ToDtoList();

    public List<NeighborhoodDto> GetByCoordinator(long coordinatorId)
        => _repository.GetByCoordinator(coordinatorId).ToDtoList();

    public long Create(string name, string description, long cityId, long coordinatorId)
        => _repository.Create(name, description, cityId, coordinatorId);

    public void AddStreet(long neighborhoodId, string streetName, int startNumber, int endNumber)
        => _repository.AddStreet(neighborhoodId, streetName, startNumber, endNumber);

    public void AddImage(long neighborhoodId, string imagePath)
        => _repository.AddImage(neighborhoodId, imagePath);
    public string? GetNameById(long neighborhoodId)
    => _repository.GetNameById(neighborhoodId);
    public (long coordinatorId, string coordinatorName) GetCoordinatorInfo(long neighborhoodId)
    => _repository.GetCoordinatorInfo(neighborhoodId);
}
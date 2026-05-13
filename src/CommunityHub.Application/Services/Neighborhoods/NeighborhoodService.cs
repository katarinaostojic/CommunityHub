using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

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
}
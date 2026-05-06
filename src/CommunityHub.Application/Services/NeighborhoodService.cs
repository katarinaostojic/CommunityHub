using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Services;

public class NeighborhoodService
{
    private readonly NeighborhoodDbRepository _repository;

    public NeighborhoodService()
    {
        _repository = new NeighborhoodDbRepository();
    }

    public List<Neighborhood> SearchForCitizen(string? name, string? address, string? city, string? country)
    {
        return _repository.SearchForCitizen(name, address, city, country);
    }
}
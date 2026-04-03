using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services.TenantServices;

public class BuildingService
{
    private readonly BuildingDbRepository _repository;

    public BuildingService()
    {
        _repository = new BuildingDbRepository();
    }

    public List<Building> GetAll()
    {
        return _repository.GetAll();
    }

    public List<Building> Search(string? street, string? neighborhood, string? city, string? country)
    {
        return _repository.Search(street, neighborhood, city, country);
    }
}
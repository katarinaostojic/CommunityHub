using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services.TenantServices;

using CommunityHub.Application.Database.Repositories;

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

    public int GetVacancies(Building building)
    {
        List<string> occupiedUnits = _repository.GetOccupiedUnits(building.Id);
        return building.TotalUnits - occupiedUnits.Count;
    }
}
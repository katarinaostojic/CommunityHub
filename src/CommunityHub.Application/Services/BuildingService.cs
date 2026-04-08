using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services;

public class BuildingService
{
    private readonly BuildingDbRepository _repository;

    public BuildingService()
    {
        _repository = new BuildingDbRepository();
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

    public List<BuildingMembership> GetMembershipsByTenant(long tenantId)
    {
        return _repository.GetMembershipsByTenant(tenantId);
    }

    public Building? GetById(long buildingId)
    {
        return _repository.GetById(buildingId);
    }

    public long CreateBuilding(string street, string streetNumber, string neighborhood, long cityId, int numberOfFloors, long managerId)
    {
        return _repository.CreateBuilding(street, streetNumber, neighborhood, cityId, numberOfFloors, managerId);
    }

    public long CreateFloorReturningId(long buildingId, int floorNumber)
    {
        return _repository.CreateFloorReturningId(buildingId, floorNumber);
    }

    public void CreateUnit(long floorId, string unitNumber)
    {
        _repository.CreateUnit(floorId, unitNumber);
    }

    public List<Building> GetAllByManager(long managerId)
    {
        return _repository.GetAllByManager(managerId);
    }

    public void SaveBuildingImage(long buildingId, string path)
    {
        ImageDbRepository imageRepository = new ImageDbRepository();
        imageRepository.SaveImage("building", buildingId, path);
    }
}
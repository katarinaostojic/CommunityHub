using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services.ManagerServices;

public class ManagerBuildingService
{
    private readonly BuildingDbRepository _repository;

    public ManagerBuildingService()
    {
        _repository = new BuildingDbRepository();
    }

    public long CreateBuilding(string street, string streetNumber, string neighborhood, long cityId, int numberOfFloors)
    {
        return _repository.CreateBuilding(street, streetNumber, neighborhood, cityId, numberOfFloors);
    }

    public long CreateFloorReturningId(long buildingId, int floorNumber)
    {
        return _repository.CreateFloorReturningId(buildingId, floorNumber);
    }

    public void CreateUnit(long floorId, string unitNumber)
    {
        _repository.CreateUnit(floorId, unitNumber);
    }
}
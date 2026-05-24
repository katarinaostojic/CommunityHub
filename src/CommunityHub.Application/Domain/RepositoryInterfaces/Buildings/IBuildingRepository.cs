using CommunityHub.Application.Domain.Entities.Buildings;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;

public interface IBuildingRepository
{
    List<Building> Search(string? street, string? neighborhood, string? city, string? country);
    Building? GetById(long buildingId);
    List<Building> GetAllByManager(long managerId);
    long CreateBuilding(string street, string streetNumber, string neighborhood, long cityId, int numberOfFloors, long managerId);
    long CreateFloor(long buildingId, int floorNumber);
    void CreateUnit(long floorId, string unitNumber);
    bool BuildingExists(string street, string streetNumber, long cityId);
}
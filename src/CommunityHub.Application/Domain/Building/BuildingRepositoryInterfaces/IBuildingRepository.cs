namespace CommunityHub.Application.Domain.Building.BuildingRepositoryInterfaces;

public interface IBuildingRepository
{
    List<Building> Search(string? street, string? neighborhood, string? city, string? country);
    Building? GetById(long buildingId);
    List<Building> GetAllByManager(long managerId);
    long CreateBuilding(string street, string streetNumber, string neighborhood, long cityId, int numberOfFloors, long managerId);
    long CreateFloorReturningId(long buildingId, int floorNumber);
    void CreateUnit(long floorId, string unitNumber);
    bool BuildingExists(string street, string streetNumber, long cityId);
}
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Services.Interfaces.Buildings;

public interface IBuildingService
{
    List<BuildingDto> Search(string? street, string? neighborhood, string? city, string? country);

    BuildingDto? GetById(long buildingId);

    List<BuildingDto> GetAllByManager(long managerId);

    long CreateBuilding(
        string street,
        string streetNumber,
        string neighborhood,
        long cityId,
        int numberOfFloors,
        long managerId);

    long CreateFloor(long buildingId, int floorNumber);

    void CreateUnit(long floorId, string unitNumber);

    void SaveBuildingImage(long buildingId, string path);

    bool BuildingExists(string street, string streetNumber, long cityId);

    List<string> GetSortedUnitNumbers(long buildingId);

    bool IsUnitOccupied(long buildingId, string unitNumber);

    bool ContainsUnit(long buildingId, string unitNumber);

    bool HasExistingRequest(long buildingId, long userId, string unitNumber);
}
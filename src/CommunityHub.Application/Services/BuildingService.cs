using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Domain.Building.BuildingRepositoryInterfaces;

namespace CommunityHub.Application.Services;

public class BuildingService
{
    private readonly IBuildingRepository _repository;
    private readonly IImageRepository _imageRepository;

    public BuildingService(IBuildingRepository repository, IImageRepository imageRepository)
    {
        _repository = repository;
        _imageRepository = imageRepository;
    }

    public List<Building> Search(string? street, string? neighborhood, string? city, string? country)
    {
        return _repository.Search(street, neighborhood, city, country);
    }

    public Building? GetById(long buildingId)
    {
        return _repository.GetById(buildingId);
    }

    public int GetVacancies(Building building)
    {
        return building.TotalUnits - building.Memberships.Count;
    }

    public List<Building> GetAllByManager(long managerId)
    {
        return _repository.GetAllByManager(managerId);
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

    public void SaveBuildingImage(long buildingId, string path)
    {
        _imageRepository.SaveImage("building", buildingId, path);
    }

    public bool BuildingExists(string street, string streetNumber, long cityId)
    {
        return _repository.BuildingExists(street, streetNumber, cityId);
    }
}
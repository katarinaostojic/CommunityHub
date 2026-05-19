using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Mappings.Buildings;

namespace CommunityHub.Application.Services.Buildings;

public class BuildingService
{
    private readonly IBuildingRepository _repository;
    private readonly IImageRepository _imageRepository;

    public BuildingService(IBuildingRepository repository, IImageRepository imageRepository)
    {
        _repository = repository;
        _imageRepository = imageRepository;
    }

    public List<BuildingDto> Search(string? street, string? neighborhood, string? city, string? country)
    {
        return _repository.Search(street, neighborhood, city, country).ToDtoList();
    }

    public BuildingDto? GetById(long buildingId)
    {
        return _repository.GetById(buildingId)?.ToDto();
    }

    public List<BuildingDto> GetAllByManager(long managerId)
    {
        return _repository.GetAllByManager(managerId).ToDtoList();
    }

    public long CreateBuilding(string street, string streetNumber, string neighborhood, long cityId, int numberOfFloors, long managerId)
    {
        return _repository.CreateBuilding(street, streetNumber, neighborhood, cityId, numberOfFloors, managerId);
    }

    public long CreateFloor(long buildingId, int floorNumber)
    {
        return _repository.CreateFloor(buildingId, floorNumber);
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

    public List<string> GetSortedUnitNumbers(long buildingId)
    {
        Building? building = _repository.GetById(buildingId);
        return building?.GetSortedUnitNumbers() ?? new List<string>();
    }

    public bool IsUnitOccupied(long buildingId, string unitNumber)
    {
        Building? building = _repository.GetById(buildingId);
        return building?.IsUnitOccupied(unitNumber) ?? false;
    }

    public bool ContainsUnit(long buildingId, string unitNumber)
    {
        Building? building = _repository.GetById(buildingId);
        return building?.ContainsUnit(unitNumber) ?? false;
    }

    public bool HasExistingRequest(long buildingId, long userId, string unitNumber)
    {
        Building? building = _repository.GetById(buildingId);
        return building?.HasExistingRequest(userId, unitNumber) ?? false;
    }
}
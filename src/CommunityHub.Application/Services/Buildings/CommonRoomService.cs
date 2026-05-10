using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;

namespace CommunityHub.Application.Services.Buildings;

public class CommonRoomService
{
    private readonly ICommonRoomRepository _repository;
    private readonly IBuildingRepository _buildingRepository;

    public CommonRoomService(ICommonRoomRepository repository, IBuildingRepository buildingRepository)
    {
        _repository = repository;
        _buildingRepository = buildingRepository;
    }

    public List<CommonRoom> GetByBuilding(long buildingId)
    {
        return _repository.GetByBuilding(buildingId);
    }

    public void Create(string name, string description, int floorNumber,
                       RentalType rentalType, long buildingId)
    {
        Building? building = _buildingRepository.GetById(buildingId);
        if (building == null)
            throw new Exception("Building not found.");

        CommonRoom commonRoom = new CommonRoom(0, name, description, floorNumber, rentalType, buildingId);
        if (!commonRoom.IsFloorValid(building.NumberOfFloors))
            throw new Exception("Floor does not exist in this building.");

        string rentalTypeString = rentalType == RentalType.PerDay ? "per_day" : "multi_day";
        _repository.Create(name, description, floorNumber, rentalTypeString, buildingId);
    }

    public CommonRoom? GetCalendar(long roomId)
    {
        CommonRoom? room = _repository.GetById(roomId);
        if (room == null) return null;

        List<DateTime> occupiedDates = _repository.GetOccupiedDates(roomId);
        room.SetOccupiedDates(occupiedDates);
        return room;
    }
}
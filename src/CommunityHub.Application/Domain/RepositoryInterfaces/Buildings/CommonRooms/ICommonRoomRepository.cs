using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;

public interface ICommonRoomRepository
{
    List<CommonRoom> GetByBuilding(long buildingId);
    long Create(string name, string description, int floorNumber, string rentalType, long buildingId);
    List<DateTime> GetOccupiedDates(long roomId);
    CommonRoom? GetById(long roomId);
    void BookDate(long commonRoomId, DateTime date);
}
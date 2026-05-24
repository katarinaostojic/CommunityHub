using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Interfaces.Buildings.CommonRooms;

public interface ICommonRoomService
{
    List<CommonRoomDto> GetByBuilding(long buildingId);

    void Create(
        string name,
        string description,
        int floorNumber,
        RentalType rentalType,
        long buildingId);

    List<DateTime> GetOccupiedDates(long commonRoomId);
}
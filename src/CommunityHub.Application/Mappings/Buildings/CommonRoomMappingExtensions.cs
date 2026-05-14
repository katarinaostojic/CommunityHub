using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Mappings.Buildings;

public static class CommonRoomMappingExtensions
{
    public static CommonRoomDto ToDto(this CommonRoom room)
    {
        return new CommonRoomDto(
            id: room.Id,
            name: room.Name,
            description: room.Description,
            floorNumber: room.FloorNumber,
            rentalType: room.RentalType,
            buildingId: room.BuildingId
        );
    }

    public static List<CommonRoomDto> ToDtoList(this IEnumerable<CommonRoom> rooms)
    {
        return rooms.Select(r => r.ToDto()).ToList();
    }
}

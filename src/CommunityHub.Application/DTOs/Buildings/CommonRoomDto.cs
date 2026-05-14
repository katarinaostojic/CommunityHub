using CommunityHub.Application.Domain.Buildings;

namespace CommunityHub.Application.DTOs.Buildings;

public class CommonRoomDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int FloorNumber { get; init; }
    public RentalType RentalType { get; init; }
    public long BuildingId { get; init; }

    public CommonRoomDto(
        long id,
        string name,
        string description,
        int floorNumber,
        RentalType rentalType,
        long buildingId)
    {
        Id = id;
        Name = name;
        Description = description;
        FloorNumber = floorNumber;
        RentalType = rentalType;
        BuildingId = buildingId;
    }

    public string RentalTypeDisplay => RentalType == RentalType.PerDay ? "Per day" : "Multiple days";
}
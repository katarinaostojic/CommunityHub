using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

namespace CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;

public static class RentalTypeMapper
{
    public static string ToDatabaseValue(RentalType rentalType)
    {
        return rentalType switch
        {
            RentalType.PerDay => "per_day",
            RentalType.MultiDay => "multi_day",
            _ => throw new ArgumentException($"Unknown rental type: {rentalType}")
        };
    }

    public static RentalType FromDatabaseValue(string value)
    {
        return value switch
        {
            "per_day" => RentalType.PerDay,
            "multi_day" => RentalType.MultiDay,
            _ => throw new ArgumentException($"Unknown rental type: {value}")
        };
    }
}
using CommunityHub.Application.Domain.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings;

public static class CommonRoomMapper
{
    public static CommonRoom Map(IDataReader reader)
    {
        RentalType rentalType = reader["rental_type"].ToString() == "per_day"
            ? RentalType.PerDay
            : RentalType.MultiDay;

        return new CommonRoom(
            Convert.ToInt64(reader["id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            Convert.ToInt32(reader["floor_number"]),
            rentalType,
            Convert.ToInt64(reader["building_id"])
        );
    }
}
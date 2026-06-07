using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;

public static class CommonRoomMapper
{
    public static CommonRoom Map(IDataReader reader)
    {
        RentalType rentalType = RentalTypeMapper.FromDatabaseValue(
            reader["rental_type"].ToString()!);

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
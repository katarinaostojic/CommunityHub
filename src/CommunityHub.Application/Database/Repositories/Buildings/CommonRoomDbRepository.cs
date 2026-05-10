using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings;

public class CommonRoomDbRepository : BaseDbRepository, ICommonRoomRepository
{
    public List<CommonRoom> GetByBuilding(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, description, floor_number, rental_type, building_id
            FROM common_rooms
            WHERE building_id = @buildingId
            ORDER BY floor_number, name";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        List<CommonRoom> rooms = new List<CommonRoom>();
        while (reader.Read())
            rooms.Add(CommonRoomMapper.Map(reader));
        return rooms;
    }

    public long Create(string name, string description, int floorNumber,
                       string rentalType, long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO common_rooms (name, description, floor_number, rental_type, building_id)
            VALUES (@name, @description, @floorNumber, @rentalType::rental_type, @buildingId)
            RETURNING id";

        AddParameter(command, "@name", name);
        AddParameter(command, "@description", description);
        AddParameter(command, "@floorNumber", floorNumber);
        AddParameter(command, "@rentalType", rentalType);
        AddParameter(command, "@buildingId", buildingId);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public List<DateTime> GetOccupiedDates(long roomId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT booked_date
            FROM common_room_bookings
            WHERE common_room_id = @roomId";

        AddParameter(command, "@roomId", roomId);

        using IDataReader reader = command.ExecuteReader();
        List<DateTime> dates = new List<DateTime>();
        while (reader.Read())
            dates.Add(Convert.ToDateTime(reader["booked_date"]));
        return dates;
    }

    public CommonRoom? GetById(long roomId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, name, description, floor_number, rental_type, building_id
        FROM common_rooms
        WHERE id = @roomId";

        AddParameter(command, "@roomId", roomId);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
            return CommonRoomMapper.Map(reader);
        return null;
    }
}
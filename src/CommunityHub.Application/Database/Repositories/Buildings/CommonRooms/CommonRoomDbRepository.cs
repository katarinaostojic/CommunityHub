using CommunityHub.Application.Database.Readers.Buildings.CommonRooms;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces.CommonRoomRepositoryInterfaces;
using CommunityHub.Application.Domain.Buildings.CommonRooms;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.CommonRooms;

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
        return CommonRoomReader.ReadRooms(reader);
    }

    public long Create(
        string name,
        string description,
        int floorNumber,
        string rentalType,
        long buildingId)
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
        return CommonRoomReader.ReadOccupiedDates(reader);
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
        return CommonRoomReader.ReadSingleRoom(reader);
    }

    public void BookDate(long commonRoomId, DateTime date)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO common_room_bookings (common_room_id, booked_date)
        VALUES (@commonRoomId, @date)";

        AddParameter(command, "@commonRoomId", commonRoomId);
        AddParameter(command, "@date", date);

        command.ExecuteNonQuery();
    }
}
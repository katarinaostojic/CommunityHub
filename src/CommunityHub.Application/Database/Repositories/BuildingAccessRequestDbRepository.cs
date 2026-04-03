using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingAccessRequestDbRepository
{
    public bool IsUnitOccupied(long buildingId, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM building_memberships
            WHERE building_id = @buildingId AND unit_number = @unitNumber";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@unitNumber", unitNumber);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public void Create(long userId, long buildingId, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO building_access_requests (user_id, building_id, unit_number, created_at, status)
            VALUES (@userId, @buildingId, @unitNumber, @createdAt, 'pending approval')";

        AddParameter(command, "@userId", userId);
        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@unitNumber", unitNumber);
        AddParameter(command, "@createdAt", DateTime.Today);

        command.ExecuteNonQuery();
    }

    public List<BuildingAccessRequest> GetAllByTenant(long userId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT r.id, r.unit_number, r.created_at, r.status, r.rejection_reason,
                   b.id AS building_id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_access_requests r
            JOIN buildings b ON r.building_id = b.id
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            JOIN users u ON r.user_id = u.id
            WHERE r.user_id = @userId
            ORDER BY r.created_at DESC";

        AddParameter(command, "@userId", userId);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public void Delete(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM building_access_requests WHERE id = @id";

        AddParameter(command, "@id", requestId);

        command.ExecuteNonQuery();
    }

    private List<BuildingAccessRequest> ReadRequests(IDataReader reader)
    {
        List<BuildingAccessRequest> requests = new List<BuildingAccessRequest>();

        while (reader.Read())
            requests.Add(MapRequest(reader));

        return requests;
    }

    private BuildingAccessRequest MapRequest(IDataReader reader)
    {
        Building building = MapBuilding(reader);
        User user = MapUser(reader);
        string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
            ? null
            : reader["rejection_reason"].ToString();

        return new BuildingAccessRequest(
            Convert.ToInt64(reader["id"]),
            user,
            building,
            reader["unit_number"].ToString(),
            DateTime.Parse(reader["created_at"].ToString()),
            reader["status"].ToString(),
            rejectionReason
        );
    }

    private Building MapBuilding(IDataReader reader)
    {
        Country country = new Country(
            Convert.ToInt64(reader["country_id"]),
            reader["country_name"].ToString(),
            reader["country_code"].ToString()
        );

        City city = new City(
            Convert.ToInt64(reader["city_id"]),
            reader["city_name"].ToString(),
            country
        );

        return new Building(
            Convert.ToInt64(reader["building_id"]),
            reader["street"].ToString(),
            reader["street_number"].ToString(),
            reader["neighborhood"].ToString(),
            city,
            Convert.ToInt32(reader["number_of_floors"])
        );
    }

    private User MapUser(IDataReader reader)
    {
        return new User(
            Convert.ToInt64(reader["user_id"]),
            reader["username"].ToString(),
            reader["password"].ToString(),
            reader["name"].ToString(),
            reader["surname"].ToString(),
            DateTime.Parse(reader["birthday"].ToString()),
            reader["role"].ToString()
        );
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        command.Parameters.Add(param);
    }
}
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

        IDbDataParameter buildingIdParam = command.CreateParameter();
        buildingIdParam.ParameterName = "@buildingId";
        buildingIdParam.Value = buildingId;
        command.Parameters.Add(buildingIdParam);

        IDbDataParameter unitNumberParam = command.CreateParameter();
        unitNumberParam.ParameterName = "@unitNumber";
        unitNumberParam.Value = unitNumber;
        command.Parameters.Add(unitNumberParam);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public void Create(long userId, long buildingId, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO building_access_requests (user_id, building_id, unit_number, created_at, status)
            VALUES (@userId, @buildingId, @unitNumber, @createdAt, 'pending approval')";

        IDbDataParameter userIdParam = command.CreateParameter();
        userIdParam.ParameterName = "@userId";
        userIdParam.Value = userId;
        command.Parameters.Add(userIdParam);

        IDbDataParameter buildingIdParam = command.CreateParameter();
        buildingIdParam.ParameterName = "@buildingId";
        buildingIdParam.Value = buildingId;
        command.Parameters.Add(buildingIdParam);

        IDbDataParameter unitNumberParam = command.CreateParameter();
        unitNumberParam.ParameterName = "@unitNumber";
        unitNumberParam.Value = unitNumber;
        command.Parameters.Add(unitNumberParam);

        IDbDataParameter createdAtParam = command.CreateParameter();
        createdAtParam.ParameterName = "@createdAt";
        createdAtParam.Value = DateTime.Today;
        command.Parameters.Add(createdAtParam);

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

        IDbDataParameter userIdParam = command.CreateParameter();
        userIdParam.ParameterName = "@userId";
        userIdParam.Value = userId;
        command.Parameters.Add(userIdParam);

        using IDataReader reader = command.ExecuteReader();

        List<BuildingAccessRequest> requests = new List<BuildingAccessRequest>();

        while (reader.Read())
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

            Building building = new Building(
                Convert.ToInt64(reader["building_id"]),
                reader["street"].ToString(),
                reader["street_number"].ToString(),
                reader["neighborhood"].ToString(),
                city,
                Convert.ToInt32(reader["number_of_floors"])
            );

            User user = new User(
                Convert.ToInt64(reader["user_id"]),
                reader["username"].ToString(),
                reader["password"].ToString(),
                reader["name"].ToString(),
                reader["surname"].ToString(),
                DateTime.Parse(reader["birthday"].ToString()),
                reader["role"].ToString()
            );

            string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
                ? null
                : reader["rejection_reason"].ToString();

            requests.Add(new BuildingAccessRequest(
                Convert.ToInt64(reader["id"]),
                user,
                building,
                reader["unit_number"].ToString(),
                DateTime.Parse(reader["created_at"].ToString()),
                reader["status"].ToString(),
                rejectionReason
            ));
        }

        return requests;
    }

    public void Delete(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM building_access_requests WHERE id = @id";

        IDbDataParameter idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = requestId;
        command.Parameters.Add(idParam);

        command.ExecuteNonQuery();
    }
}
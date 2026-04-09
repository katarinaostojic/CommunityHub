using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingAccessRequestDbRepository : BaseDbRepository
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

    public void Create(User user, Building building, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO building_access_requests (user_id, building_id, unit_number, created_at, status)
            VALUES (@userId, @buildingId, @unitNumber, @createdAt, 'pending approval')";

        AddParameter(command, "@userId", user.Id);
        AddParameter(command, "@buildingId", building.Id);
        AddParameter(command, "@unitNumber", unitNumber);
        AddParameter(command, "@createdAt", DateTime.UtcNow);

        command.ExecuteNonQuery();
    }

    //ne moze u istoj zgradi za isti stan da posalje zahtev opet
    public bool HasExistingRequest(User user, Building building, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM building_access_requests
        WHERE user_id = @userId AND building_id = @buildingId AND unit_number = @unitNumber";

        AddParameter(command, "@userId", user.Id);
        AddParameter(command, "@buildingId", building.Id);
        AddParameter(command, "@unitNumber", unitNumber);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public List<BuildingAccessRequest> GetAllByTenant(long tenantId, string? status, bool sortDescending)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = $@"
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
          AND (@status IS NULL OR r.status = @status)
        ORDER BY r.created_at {(sortDescending ? "DESC" : "ASC")}";

        AddParameter(command, "@userId", tenantId);
        AddParameter(command, "@status", status);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public int CountByTenantAndStatus(long tenantId, string? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM building_access_requests
        WHERE user_id = @userId
          AND (@status IS NULL OR status = @status)";

        AddParameter(command, "@userId", tenantId);
        AddParameter(command, "@status", status);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Delete(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM building_access_requests WHERE id = @id";

        AddParameter(command, "@id", requestId);

        command.ExecuteNonQuery();
    }

    public int GetPendingRequestsCount(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM building_access_requests
            WHERE building_id = @buildingId AND status = 'pending approval'";

        AddParameter(command, "@buildingId", buildingId);

        return Convert.ToInt32(command.ExecuteScalar());
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
        string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
            ? null
            : reader["rejection_reason"].ToString();

        return new BuildingAccessRequest(
            Convert.ToInt64(reader["id"]),
            UserMapper.Map(reader),
            BuildingMapper.MapFromJoin(reader),
            reader["unit_number"].ToString()!,
            DateTime.Parse(reader["created_at"].ToString()!),
            RequestStatusMapper.Parse(reader["status"].ToString()!),
            rejectionReason
        );
    }

    public List<BuildingAccessRequest> GetAllByManager(long managerId, string? status, bool sortDescending)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = $@"
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
    WHERE b.manager_id = @managerId
      AND (@status IS NULL OR r.status = @status::request_status)
    ORDER BY r.created_at {(sortDescending ? "DESC" : "ASC")}";

        AddParameter(command, "@managerId", managerId);
        AddParameter(command, "@status", status);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public void ApproveRequest(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE building_access_requests SET status = 'accepted'::request_status WHERE id = @id";
        AddParameter(command, "@id", requestId);
        command.ExecuteNonQuery();
    }

    public void RejectRequest(long requestId, string? rejectionReason)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE building_access_requests
        SET status = 'rejected'::request_status, rejection_reason = @reason
        WHERE id = @id";
        AddParameter(command, "@id", requestId);
        AddParameter(command, "@reason", rejectionReason);
        command.ExecuteNonQuery();
    }

    public void CreateMembership(BuildingAccessRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand floorCmd = connection.CreateCommand();
        floorCmd.CommandText = @"
        SELECT f.floor_number FROM units u
        JOIN floors f ON u.floor_id = f.id
        WHERE f.building_id = @buildingId AND u.unit_number = @unitNumber
        LIMIT 1";
        AddParameter(floorCmd, "@buildingId", request.Building.Id);
        AddParameter(floorCmd, "@unitNumber", request.UnitNumber);

        object? floorResult = floorCmd.ExecuteScalar();
        int floorNumber = floorResult != null && floorResult != DBNull.Value
            ? Convert.ToInt32(floorResult)
            : 0;

        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO building_memberships (building_id, user_id, unit_number, floor_number, approved_at)
        VALUES (@buildingId, @userId, @unitNumber, @floorNumber, @approvedAt)";
        AddParameter(cmd, "@buildingId", request.Building.Id);
        AddParameter(cmd, "@userId", request.User.Id);
        AddParameter(cmd, "@unitNumber", request.UnitNumber);
        AddParameter(cmd, "@floorNumber", floorNumber);
        AddParameter(cmd, "@approvedAt", DateTime.UtcNow);
        cmd.ExecuteNonQuery();
    }
}
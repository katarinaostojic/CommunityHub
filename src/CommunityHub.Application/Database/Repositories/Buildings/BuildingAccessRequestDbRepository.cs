using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings;

public class BuildingAccessRequestDbRepository : BaseDbRepository, IBuildingAccessRequestRepository
{
    public void Create(BuildingAccessRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO building_access_requests (user_id, building_id, unit_number, created_at, status)
            VALUES (@userId, @buildingId, @unitNumber, @createdAt, 'pending approval')";

        AddParameter(command, "@userId", request.Tenant.Id);
        AddParameter(command, "@buildingId", request.Building.Id);
        AddParameter(command, "@unitNumber", request.UnitNumber);
        AddParameter(command, "@createdAt", request.CreatedAt);

        command.ExecuteNonQuery();
    }

    public BuildingAccessRequest? GetById(long requestId)
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
        WHERE r.id = @id";

        AddParameter(command, "@id", requestId);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read()
            ? BuildingAccessRequestMapper.MapWithBuilding(reader)
            : null;
    }

    public List<BuildingAccessRequest> GetAllByTenant(long tenantId, RequestStatus? status, bool sortDescending)
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
              AND (@status IS NULL OR r.status = @status::request_status)
            ORDER BY r.created_at {(sortDescending ? "DESC" : "ASC")}";

        AddParameter(command, "@userId", tenantId);
        AddParameter(command, "@status", status.HasValue ? RequestStatusMapper.ToDbString(status.Value) : null);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public int CountByTenantAndStatus(long tenantId, RequestStatus? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM building_access_requests
            WHERE user_id = @userId
              AND (@status IS NULL OR status = @status::request_status)";

        AddParameter(command, "@userId", tenantId);
        AddParameter(command, "@status", status.HasValue ? RequestStatusMapper.ToDbString(status.Value) : null);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int GetPendingRequestsCount(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM building_access_requests
            WHERE building_id = @buildingId AND status = 'pending approval'::request_status";

        AddParameter(command, "@buildingId", buildingId);

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

    public void Update(BuildingAccessRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE building_access_requests
            SET status = @status::request_status, rejection_reason = @reason
            WHERE id = @id";

        AddParameter(command, "@id", request.Id);
        AddParameter(command, "@status", RequestStatusMapper.ToDbString(request.Status));
        AddParameter(command, "@reason", request.RejectionReason);

        command.ExecuteNonQuery();
    }

    private List<BuildingAccessRequest> ReadRequests(IDataReader reader)
    {
        List<BuildingAccessRequest> requests = new List<BuildingAccessRequest>();
        while (reader.Read())
            requests.Add(BuildingAccessRequestMapper.MapWithBuilding(reader));
        return requests;
    }
}
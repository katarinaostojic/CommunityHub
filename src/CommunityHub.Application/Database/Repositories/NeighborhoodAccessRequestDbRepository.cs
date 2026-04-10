using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class NeighborhoodAccessRequestDbRepository : BaseDbRepository
{
    public void Create(User citizen, Neighborhood neighborhood)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO neighborhood_access_requests (citizen_id, neighborhood_id, created_at, status, rejection_reason)
            VALUES (@citizenId, @neighborhoodId, @createdAt, 'pending approval', NULL)";

        AddParameter(command, "@citizenId", citizen.Id);
        AddParameter(command, "@neighborhoodId", neighborhood.Id);
        AddParameter(command, "@createdAt", DateTime.UtcNow);

        command.ExecuteNonQuery();
    }

    public bool HasExistingPendingRequest(User citizen, Neighborhood neighborhood)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM neighborhood_access_requests
            WHERE citizen_id = @citizenId 
              AND neighborhood_id = @neighborhoodId
              AND status = 'pending approval'";

        AddParameter(command, "@citizenId", citizen.Id);
        AddParameter(command, "@neighborhoodId", neighborhood.Id);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public bool HasMembership(long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM neighborhood_memberships
            WHERE citizen_id = @citizenId";

        AddParameter(command, "@citizenId", citizenId);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public List<NeighborhoodAccessRequest> GetAllByCitizen(long citizenId, string? status, bool sortDescending)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = $@"
        SELECT r.id, r.created_at, r.status, r.rejection_reason,
               n.id AS neighborhood_id, n.name, n.description, n.location_id, n.coordinator_id, n.budget,
               l.city_name, l.country_name,
               u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role, u.address_id
        FROM neighborhood_access_requests r
        JOIN neighborhoods n ON r.neighborhood_id = n.id
        JOIN locations l ON n.location_id = l.id
        JOIN users u ON r.citizen_id = u.id
        WHERE r.citizen_id = @citizenId
          AND (@status IS NULL OR r.status = @status)
        ORDER BY r.created_at {(sortDescending ? "DESC" : "ASC")}";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@status", status);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public int CountByCitizenAndStatus(long citizenId, string? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM neighborhood_access_requests
        WHERE citizen_id = @citizenId
          AND (@status IS NULL OR status = @status)";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@status", status);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Delete(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM neighborhood_access_requests WHERE id = @id";

        AddParameter(command, "@id", requestId);

        command.ExecuteNonQuery();
    }

    public void ApproveRequest(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE neighborhood_access_requests
            SET status = 'approved', rejection_reason = NULL
            WHERE id = @id";

        AddParameter(command, "@id", requestId);
        command.ExecuteNonQuery();
    }

    public void RejectRequest(long requestId, string? rejectionReason)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE neighborhood_access_requests
        SET status = 'rejected', rejection_reason = @reason
        WHERE id = @id";

        AddParameter(command, "@id", requestId);
        AddParameter(command, "@reason", rejectionReason);

        command.ExecuteNonQuery();
    }

    public void CreateMembership(NeighborhoodAccessRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO neighborhood_memberships (citizen_id, neighborhood_id, joined_at)
        VALUES (@citizenId, @neighborhoodId, @joinedAt)";

        AddParameter(command, "@citizenId", request.Citizen.Id);
        AddParameter(command, "@neighborhoodId", request.Neighborhood.Id);
        AddParameter(command, "@joinedAt", DateTime.UtcNow);

        command.ExecuteNonQuery();
    }

    private List<NeighborhoodAccessRequest> ReadRequests(IDataReader reader)
    {
        List<NeighborhoodAccessRequest> requests = new List<NeighborhoodAccessRequest>();
        while (reader.Read())
            requests.Add(MapRequest(reader));
        return requests;
    }

    private NeighborhoodAccessRequest MapRequest(IDataReader reader)
    {
        string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
            ? null
            : reader["rejection_reason"].ToString();

        return new NeighborhoodAccessRequest(
            Convert.ToInt64(reader["id"]),
            UserMapper.Map(reader),
            NeighborhoodMapper.MapFromJoin(reader),
            DateTime.Parse(reader["created_at"].ToString()!),
            RequestStatusMapper.Parse(reader["status"].ToString()!),
            rejectionReason
        );
    }
}
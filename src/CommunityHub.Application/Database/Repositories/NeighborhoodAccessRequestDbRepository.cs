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
        IDbCommand command = BuildGetAllByCitizenCommand(connection, citizenId, status, sortDescending);
        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    private IDbCommand BuildGetAllByCitizenCommand(IDbConnection connection, long citizenId, string? status, bool sortDescending)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = $@"
            SELECT r.id, r.created_at, r.status, r.rejection_reason,
                   u.id AS citizen_id, u.username, u.password,
                   u.name AS citizen_name, u.surname AS citizen_surname,
                   u.birthday, u.role, u.address,
                   n.id AS neighborhood_id, n.name AS neighborhood_name,
                   n.description, n.city_id, n.coordinator_id, n.budget,
                   c.name AS city_name, co.name AS country_name
            FROM neighborhood_access_requests r
            JOIN neighborhoods n ON r.neighborhood_id = n.id
            JOIN cities c ON n.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            JOIN users u ON r.citizen_id = u.id
            WHERE r.citizen_id = @citizenId
              AND (@status IS NULL OR r.status::text = @status)
            ORDER BY r.created_at {(sortDescending ? "DESC" : "ASC")}";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@status", status);
        return command;
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
        List<NeighborhoodAccessRequest> requests = new();
        while (reader.Read())
            requests.Add(MapRequest(reader));
        return requests;
    }

    private NeighborhoodAccessRequest MapRequest(IDataReader reader)
    {
        User citizen = MapCitizen(reader);
        Neighborhood neighborhood = MapNeighborhood(reader);
        string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
            ? null : reader["rejection_reason"].ToString();

        return new NeighborhoodAccessRequest(
            Convert.ToInt64(reader["id"]),
            citizen,
            neighborhood,
            Convert.ToDateTime(reader["created_at"]),
            ParseRequestStatus(reader["status"].ToString()!),
            rejectionReason
        );
    }

    private User MapCitizen(IDataReader reader)
    {
        return new User(
            Convert.ToInt64(reader["citizen_id"]),
            reader["username"].ToString()!,
            reader["password"].ToString()!,
            reader["citizen_name"].ToString()!,
            reader["citizen_surname"].ToString()!,
            ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
            UserMapper.ParseRole(reader["role"].ToString()!),
            reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
        );
    }

    private Neighborhood MapNeighborhood(IDataReader reader)
    {
        return new Neighborhood(
            Convert.ToInt64(reader["neighborhood_id"]),
            reader["neighborhood_name"].ToString()!,
            reader["description"].ToString()!,
            new Location(
                Convert.ToInt64(reader["city_id"]),
                reader["city_name"].ToString()!,
                reader["country_name"].ToString()!
            ),
            Convert.ToDecimal(reader["budget"]),
            Convert.ToInt64(reader["coordinator_id"])
        );
    }

    private static RequestStatus ParseRequestStatus(string status)
    {
        return status.Trim().ToLower() switch
        {
            "pending approval" => RequestStatus.PendingApproval,
            "pendingapproval" => RequestStatus.PendingApproval,
            "pending_approval" => RequestStatus.PendingApproval,
            "approved" => RequestStatus.Approved,
            "accepted" => RequestStatus.Approved,
            "rejected" => RequestStatus.Rejected,
            _ => throw new ArgumentException($"Unknown status: {status}")
        };
    }
}
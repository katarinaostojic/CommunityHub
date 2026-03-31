using System.Data;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories;

public class NeighborhoodDbRepository
{
    public long Create(string name, string description, long cityId, long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO neighborhoods (name, description, city_id, budget, coordinator_id)
            VALUES (@name, @description, @cityId, 0, @coordinatorId)
            RETURNING id";

        IDbDataParameter nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = name;
        command.Parameters.Add(nameParam);

        IDbDataParameter descParam = command.CreateParameter();
        descParam.ParameterName = "@description";
        descParam.Value = description;
        command.Parameters.Add(descParam);

        IDbDataParameter cityParam = command.CreateParameter();
        cityParam.ParameterName = "@cityId";
        cityParam.Value = cityId;
        command.Parameters.Add(cityParam);

        IDbDataParameter coordParam = command.CreateParameter();
        coordParam.ParameterName = "@coordinatorId";
        coordParam.Value = coordinatorId;
        command.Parameters.Add(coordParam);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void AddStreet(long neighborhoodId, string streetName, int startNumber, int endNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO neighborhood_streets (neighborhood_id, street_name, start_number, end_number)
            VALUES (@neighborhoodId, @streetName, @startNumber, @endNumber)";

        IDbDataParameter nIdParam = command.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        command.Parameters.Add(nIdParam);

        IDbDataParameter streetParam = command.CreateParameter();
        streetParam.ParameterName = "@streetName";
        streetParam.Value = streetName;
        command.Parameters.Add(streetParam);

        IDbDataParameter startParam = command.CreateParameter();
        startParam.ParameterName = "@startNumber";
        startParam.Value = startNumber;
        command.Parameters.Add(startParam);

        IDbDataParameter endParam = command.CreateParameter();
        endParam.ParameterName = "@endNumber";
        endParam.Value = endNumber;
        command.Parameters.Add(endParam);

        command.ExecuteNonQuery();
    }

    public List<Neighborhood> GetByCoordinator(long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT n.id, n.name, n.description, n.city_id, c.name AS city_name,
                   co.name AS country_name, n.budget, n.coordinator_id,
                   s.id AS street_id, s.street_name, s.start_number, s.end_number
            FROM neighborhoods n
            JOIN cities c ON n.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN neighborhood_streets s ON s.neighborhood_id = n.id
            WHERE n.coordinator_id = @coordinatorId
            ORDER BY n.id";

        IDbDataParameter coordParam = command.CreateParameter();
        coordParam.ParameterName = "@coordinatorId";
        coordParam.Value = coordinatorId;
        command.Parameters.Add(coordParam);

        using IDataReader reader = command.ExecuteReader();

        var neighborhoods = new Dictionary<long, Neighborhood>();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);

            if (!neighborhoods.ContainsKey(id))
            {
                neighborhoods[id] = new Neighborhood(
                    id,
                    reader["name"].ToString(),
                    reader["description"].ToString(),
                    Convert.ToInt64(reader["city_id"]),
                    reader["city_name"].ToString(),
                    reader["country_name"].ToString(),
                    Convert.ToDecimal(reader["budget"]),
                    Convert.ToInt64(reader["coordinator_id"])
                );
            }

            if (!reader.IsDBNull(reader.GetOrdinal("street_id")))
            {
                neighborhoods[id].AddStreet(new Street(
                    Convert.ToInt64(reader["street_id"]),
                    id,
                    reader["street_name"].ToString(),
                    Convert.ToInt32(reader["start_number"]),
                    Convert.ToInt32(reader["end_number"])
                ));
            }
        }

        return neighborhoods.Values.ToList();
    }
    public List<NeighborhoodAccessRequest> GetRequestsByCoordinator(long coordinatorId, string? statusFilter = null)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT r.id, r.citizen_id, u.name AS citizen_name, u.surname AS citizen_surname,
               r.neighborhood_id, n.name AS neighborhood_name,
               r.created_at, r.status, r.rejection_reason
        FROM neighborhood_access_requests r
        JOIN users u ON r.citizen_id = u.id
        JOIN neighborhoods n ON r.neighborhood_id = n.id
        WHERE n.coordinator_id = @coordinatorId";

        IDbDataParameter coordParam = command.CreateParameter();
        coordParam.ParameterName = "@coordinatorId";
        coordParam.Value = coordinatorId;
        command.Parameters.Add(coordParam);

        if (!string.IsNullOrEmpty(statusFilter))
        {
            command.CommandText += " AND r.status = @status";
            IDbDataParameter statusParam = command.CreateParameter();
            statusParam.ParameterName = "@status";
            statusParam.Value = statusFilter;
            command.Parameters.Add(statusParam);
        }

        command.CommandText += " ORDER BY r.created_at DESC";

        using IDataReader reader = command.ExecuteReader();

        var requests = new List<NeighborhoodAccessRequest>();

        while (reader.Read())
        {
            requests.Add(new NeighborhoodAccessRequest(
                Convert.ToInt64(reader["id"]),
                Convert.ToInt64(reader["citizen_id"]),
                reader["citizen_name"].ToString(),
                reader["citizen_surname"].ToString(),
                Convert.ToInt64(reader["neighborhood_id"]),
                reader["neighborhood_name"].ToString(),
                Convert.ToDateTime(reader["created_at"]),
                reader["status"].ToString(),
                reader.IsDBNull(reader.GetOrdinal("rejection_reason")) ? null : reader["rejection_reason"].ToString()
            ));
        }

        return requests;
    }

    public void ApproveRequest(long requestId, long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand updateCmd = connection.CreateCommand();
        updateCmd.CommandText = "UPDATE neighborhood_access_requests SET status = 'prihvaćen' WHERE id = @id";
        IDbDataParameter idParam = updateCmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = requestId;
        updateCmd.Parameters.Add(idParam);
        updateCmd.ExecuteNonQuery();

        IDbCommand memberCmd = connection.CreateCommand();
        memberCmd.CommandText = @"
        INSERT INTO neighborhood_memberships (citizen_id, neighborhood_id, joined_at)
        VALUES (@citizenId, @neighborhoodId, CURRENT_DATE)";
        IDbDataParameter citizenParam = memberCmd.CreateParameter();
        citizenParam.ParameterName = "@citizenId";
        citizenParam.Value = citizenId;
        memberCmd.Parameters.Add(citizenParam);
        IDbDataParameter nIdParam = memberCmd.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        memberCmd.Parameters.Add(nIdParam);
        memberCmd.ExecuteNonQuery();
    }

    public void RejectRequest(long requestId, string? rejectionReason)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE neighborhood_access_requests 
        SET status = 'odbijen', rejection_reason = @reason 
        WHERE id = @id";

        IDbDataParameter idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = requestId;
        command.Parameters.Add(idParam);

        IDbDataParameter reasonParam = command.CreateParameter();
        reasonParam.ParameterName = "@reason";
        reasonParam.Value = (object?)rejectionReason ?? DBNull.Value;
        command.Parameters.Add(reasonParam);

        command.ExecuteNonQuery();
    }
}
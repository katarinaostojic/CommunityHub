using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using System.Data;

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
                    new Location(
                        Convert.ToInt64(reader["city_id"]),
                        reader["city_name"].ToString(),
                        reader["country_name"].ToString()
                    ),
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

        foreach (var neighborhood in neighborhoods.Values)
        {
            var images = GetImages(neighborhood.Id);
            foreach (var image in images)
                neighborhood.AddImage(image);
        }

        return neighborhoods.Values.ToList();
    }

    public List<NeighborhoodAccessRequest> GetRequestsByCoordinator(long coordinatorId, string? statusFilter = null)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT r.id, r.citizen_id, u.name AS citizen_name, u.surname AS citizen_surname,
               u.username, u.password, u.birthday, u.role, u.address,
               r.neighborhood_id, n.id AS n_id, n.name AS neighborhood_name,
               n.description, n.city_id, c.name AS city_name,
               co.name AS country_name, n.budget, n.coordinator_id,
               r.created_at, r.status, r.rejection_reason
        FROM neighborhood_access_requests r
        JOIN users u ON r.citizen_id = u.id
        JOIN neighborhoods n ON r.neighborhood_id = n.id
        JOIN cities c ON n.city_id = c.id
        JOIN countries co ON c.country_id = co.id
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
            User citizen = new User(
                Convert.ToInt64(reader["citizen_id"]),
                reader["username"].ToString(),
                reader["password"].ToString(),
                reader["citizen_name"].ToString(),
                reader["citizen_surname"].ToString(),
                ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
                UserMapper.ParseRole(reader["role"].ToString()!),
                reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
            );

            Neighborhood neighborhood = new Neighborhood(
                Convert.ToInt64(reader["n_id"]),
                reader["neighborhood_name"].ToString(),
                reader["description"].ToString(),
                new Location(
                    Convert.ToInt64(reader["city_id"]),
                    reader["city_name"].ToString(),
                    reader["country_name"].ToString()
                ),
                Convert.ToDecimal(reader["budget"]),
                Convert.ToInt64(reader["coordinator_id"])
            );

            requests.Add(new NeighborhoodAccessRequest(
                Convert.ToInt64(reader["id"]),
                citizen,
                neighborhood,
                Convert.ToDateTime(reader["created_at"]),
                Enum.Parse<RequestStatus>(reader["status"].ToString()),
                reader.IsDBNull(reader.GetOrdinal("rejection_reason")) ? null : reader["rejection_reason"].ToString()
            ));
        }

        return requests;
    }

    public void ApproveRequest(long requestId, long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand updateCmd = connection.CreateCommand();
        updateCmd.CommandText = "UPDATE neighborhood_access_requests SET status = 'Approved' WHERE id = @id";
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
        SET status = 'Rejected', rejection_reason = @reason 
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

    public List<Neighborhood> Search(string? name, string? address, string? city, string? country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT n.id, n.name, n.description, n.city_id, c.name AS city_name,
               co.name AS country_name, n.budget, n.coordinator_id,
               s.id AS street_id, s.street_name, s.start_number, s.end_number
        FROM neighborhoods n
        JOIN cities c ON n.city_id = c.id
        JOIN countries co ON c.country_id = co.id
        LEFT JOIN neighborhood_streets s ON s.neighborhood_id = n.id
        WHERE (@name IS NULL OR n.name ILIKE '%' || @name || '%')
          AND (@city IS NULL OR c.name ILIKE '%' || @city || '%')
          AND (@country IS NULL OR co.name ILIKE '%' || @country || '%')
          AND (@address IS NULL OR EXISTS (
              SELECT 1 FROM neighborhood_streets ns
              WHERE ns.neighborhood_id = n.id
              AND ns.street_name ILIKE '%' || @address || '%'))
        ORDER BY n.id";

        IDbDataParameter nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = (object?)name ?? DBNull.Value;
        nameParam.DbType = DbType.String;
        command.Parameters.Add(nameParam);

        IDbDataParameter addressParam = command.CreateParameter();
        addressParam.ParameterName = "@address";
        addressParam.Value = (object?)address ?? DBNull.Value;
        addressParam.DbType = DbType.String;
        command.Parameters.Add(addressParam);

        IDbDataParameter cityParam = command.CreateParameter();
        cityParam.ParameterName = "@city";
        cityParam.Value = (object?)city ?? DBNull.Value;
        cityParam.DbType = DbType.String;
        command.Parameters.Add(cityParam);

        IDbDataParameter countryParam = command.CreateParameter();
        countryParam.ParameterName = "@country";
        countryParam.Value = (object?)country ?? DBNull.Value;
        countryParam.DbType = DbType.String;
        command.Parameters.Add(countryParam);

        using IDataReader reader = command.ExecuteReader();

        Dictionary<long, Neighborhood> neighborhoods = new Dictionary<long, Neighborhood>();
        HashSet<long> addedStreets = new HashSet<long>();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);

            if (!neighborhoods.ContainsKey(id))
            {
                neighborhoods[id] = new Neighborhood(
                    id,
                    reader["name"].ToString(),
                    reader["description"].ToString(),
                    new Location(
                        Convert.ToInt64(reader["city_id"]),
                        reader["city_name"].ToString(),
                        reader["country_name"].ToString()
                    ),
                    Convert.ToDecimal(reader["budget"]),
                    Convert.ToInt64(reader["coordinator_id"])
                );
            }

            if (!reader.IsDBNull(reader.GetOrdinal("street_id")))
            {
                long streetId = Convert.ToInt64(reader["street_id"]);
                if (!addedStreets.Contains(streetId))
                {
                    addedStreets.Add(streetId);
                    neighborhoods[id].AddStreet(new Street(
                        streetId,
                        id,
                        reader["street_name"].ToString(),
                        Convert.ToInt32(reader["start_number"]),
                        Convert.ToInt32(reader["end_number"])
                    ));
                }
            }
        }

        return neighborhoods.Values.ToList();
    }

    public bool CheckAddressMatch(string streetName, int streetNumber, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM neighborhood_streets
        WHERE neighborhood_id = @neighborhoodId
        AND LOWER(street_name) = LOWER(@streetName)
        AND @streetNumber >= start_number
        AND @streetNumber <= end_number";

        IDbDataParameter nIdParam = command.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        command.Parameters.Add(nIdParam);

        IDbDataParameter streetParam = command.CreateParameter();
        streetParam.ParameterName = "@streetName";
        streetParam.Value = streetName;
        command.Parameters.Add(streetParam);

        IDbDataParameter numberParam = command.CreateParameter();
        numberParam.ParameterName = "@streetNumber";
        numberParam.Value = streetNumber;
        command.Parameters.Add(numberParam);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public void CreateMembership(long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO neighborhood_memberships (citizen_id, neighborhood_id, joined_at)
        VALUES (@citizenId, @neighborhoodId, CURRENT_DATE)";

        IDbDataParameter citizenParam = command.CreateParameter();
        citizenParam.ParameterName = "@citizenId";
        citizenParam.Value = citizenId;
        command.Parameters.Add(citizenParam);

        IDbDataParameter nIdParam = command.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        command.Parameters.Add(nIdParam);

        command.ExecuteNonQuery();
    }

    public long CreateRequest(long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO neighborhood_access_requests (citizen_id, neighborhood_id, created_at, status)
        VALUES (@citizenId, @neighborhoodId, NOW(), 'PendingApproval')
        RETURNING id";

        IDbDataParameter citizenParam = command.CreateParameter();
        citizenParam.ParameterName = "@citizenId";
        citizenParam.Value = citizenId;
        command.Parameters.Add(citizenParam);

        IDbDataParameter nIdParam = command.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        command.Parameters.Add(nIdParam);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public List<NeighborhoodAccessRequest> GetRequestsByCitizen(long citizenId, string? statusFilter = null)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT r.id, r.citizen_id, u.name AS citizen_name, u.surname AS citizen_surname,
               u.username, u.password, u.birthday, u.role, u.address,
               r.neighborhood_id, n.id AS n_id, n.name AS neighborhood_name,
               n.description, n.city_id, c.name AS city_name,
               co.name AS country_name, n.budget, n.coordinator_id,
               r.created_at, r.status, r.rejection_reason
        FROM neighborhood_access_requests r
        JOIN users u ON r.citizen_id = u.id
        JOIN neighborhoods n ON r.neighborhood_id = n.id
        JOIN cities c ON n.city_id = c.id
        JOIN countries co ON c.country_id = co.id
        WHERE r.citizen_id = @citizenId";

        IDbDataParameter citizenParam = command.CreateParameter();
        citizenParam.ParameterName = "@citizenId";
        citizenParam.Value = citizenId;
        command.Parameters.Add(citizenParam);

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
            User citizen = new User(
                Convert.ToInt64(reader["citizen_id"]),
                reader["username"].ToString(),
                reader["password"].ToString(),
                reader["citizen_name"].ToString(),
                reader["citizen_surname"].ToString(),
                ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
                UserMapper.ParseRole(reader["role"].ToString()!),
                reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
            );

            Neighborhood neighborhood = new Neighborhood(
                Convert.ToInt64(reader["n_id"]),
                reader["neighborhood_name"].ToString(),
                reader["description"].ToString(),
                new Location(
                    Convert.ToInt64(reader["city_id"]),
                    reader["city_name"].ToString(),
                    reader["country_name"].ToString()
                ),
                Convert.ToDecimal(reader["budget"]),
                Convert.ToInt64(reader["coordinator_id"])
            );

            requests.Add(new NeighborhoodAccessRequest(
                Convert.ToInt64(reader["id"]),
                citizen,
                neighborhood,
                Convert.ToDateTime(reader["created_at"]),
                Enum.Parse<RequestStatus>(reader["status"].ToString()),
                reader.IsDBNull(reader.GetOrdinal("rejection_reason")) ? null : reader["rejection_reason"].ToString()
            ));
        }

        return requests;
    }

    public void DeleteRequest(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM neighborhood_access_requests WHERE id = @id AND status = 'PendingApproval'";

        IDbDataParameter idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = requestId;
        command.Parameters.Add(idParam);

        command.ExecuteNonQuery();
    }

    public void AddImage(long neighborhoodId, string imagePath)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO images (resource, resource_id, path)
        VALUES ('neighborhood', @neighborhoodId, @path)";

        IDbDataParameter nIdParam = command.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        command.Parameters.Add(nIdParam);

        IDbDataParameter pathParam = command.CreateParameter();
        pathParam.ParameterName = "@path";
        pathParam.Value = imagePath;
        command.Parameters.Add(pathParam);

        command.ExecuteNonQuery();
    }

    public List<Image> GetImages(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, path FROM images
        WHERE resource = 'neighborhood' AND resource_id = @neighborhoodId";

        IDbDataParameter nIdParam = command.CreateParameter();
        nIdParam.ParameterName = "@neighborhoodId";
        nIdParam.Value = neighborhoodId;
        command.Parameters.Add(nIdParam);

        using IDataReader reader = command.ExecuteReader();

        var images = new List<Image>();
        while (reader.Read())
        {
            images.Add(new Image(
                Convert.ToInt64(reader["id"]),
                reader["path"].ToString()
            ));
        }

        return images;
    }
}
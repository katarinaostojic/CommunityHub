using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using System.Data;
using System.Linq;
using System.Text;

namespace CommunityHub.Application.Database.Repositories;

public class NeighborhoodDbRepository : BaseDbRepository
{
    private readonly ImageDbRepository _imageRepository = new();

    public long Create(string name, string description, long cityId, long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO neighborhoods (name, description, city_id, budget, coordinator_id)
            VALUES (@name, @description, @cityId, 0, @coordinatorId)
            RETURNING id";

        AddParameter(command, "@name", name);
        AddParameter(command, "@description", description);
        AddParameter(command, "@cityId", cityId);
        AddParameter(command, "@coordinatorId", coordinatorId);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void AddStreet(long neighborhoodId, string streetName, int startNumber, int endNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO neighborhood_streets (neighborhood_id, street_name, start_number, end_number)
            VALUES (@neighborhoodId, @streetName, @startNumber, @endNumber)";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@streetName", streetName);
        AddParameter(command, "@startNumber", startNumber);
        AddParameter(command, "@endNumber", endNumber);

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

        AddParameter(command, "@coordinatorId", coordinatorId);

        using IDataReader reader = command.ExecuteReader();

        var neighborhoods = new Dictionary<long, Neighborhood>();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);

            if (!neighborhoods.ContainsKey(id))
            {
                neighborhoods[id] = new Neighborhood(
                    id,
                    reader["name"].ToString()!,
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

            if (!reader.IsDBNull(reader.GetOrdinal("street_id")))
            {
                neighborhoods[id].AddStreet(new Street(
                    Convert.ToInt64(reader["street_id"]),
                    id,
                    reader["street_name"].ToString()!,
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

        AddParameter(command, "@coordinatorId", coordinatorId);

        if (!string.IsNullOrEmpty(statusFilter))
        {
            command.CommandText += " AND r.status::text = @status";
            AddParameter(command, "@status", statusFilter);
        }

        command.CommandText += " ORDER BY r.created_at DESC";

        using IDataReader reader = command.ExecuteReader();

        var requests = new List<NeighborhoodAccessRequest>();

        while (reader.Read())
        {
            User citizen = new User(
                Convert.ToInt64(reader["citizen_id"]),
                reader["username"].ToString()!,
                reader["password"].ToString()!,
                reader["citizen_name"].ToString()!,
                reader["citizen_surname"].ToString()!,
                ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
                UserMapper.ParseRole(reader["role"].ToString()!),
                reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
            );

            Neighborhood neighborhood = new Neighborhood(
                Convert.ToInt64(reader["n_id"]),
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

            requests.Add(new NeighborhoodAccessRequest(
                Convert.ToInt64(reader["id"]),
                citizen,
                neighborhood,
                Convert.ToDateTime(reader["created_at"]),
                ParseRequestStatus(reader["status"].ToString()!),
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
        AddParameter(updateCmd, "@id", requestId);
        updateCmd.ExecuteNonQuery();

        IDbCommand memberCmd = connection.CreateCommand();
        memberCmd.CommandText = @"
        INSERT INTO neighborhood_memberships (citizen_id, neighborhood_id, joined_at)
        VALUES (@citizenId, @neighborhoodId, CURRENT_DATE)";
        AddParameter(memberCmd, "@citizenId", citizenId);
        AddParameter(memberCmd, "@neighborhoodId", neighborhoodId);
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

        AddParameter(command, "@id", requestId);
        AddParameter(command, "@reason", rejectionReason);

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

        AddParameter(command, "@name", name);
        AddParameter(command, "@address", address);
        AddParameter(command, "@city", city);
        AddParameter(command, "@country", country);

        using IDataReader reader = command.ExecuteReader();

        Dictionary<long, Neighborhood> neighborhoods = new();
        HashSet<long> addedStreets = new();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);

            if (!neighborhoods.ContainsKey(id))
            {
                neighborhoods[id] = new Neighborhood(
                    id,
                    reader["name"].ToString()!,
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

            if (!reader.IsDBNull(reader.GetOrdinal("street_id")))
            {
                long streetId = Convert.ToInt64(reader["street_id"]);
                if (!addedStreets.Contains(streetId))
                {
                    addedStreets.Add(streetId);
                    neighborhoods[id].AddStreet(new Street(
                        streetId,
                        id,
                        reader["street_name"].ToString()!,
                        Convert.ToInt32(reader["start_number"]),
                        Convert.ToInt32(reader["end_number"])
                    ));
                }
            }
        }

        return neighborhoods.Values.ToList();
    }

    public List<Neighborhood> SearchForCitizen(string? name, string? address, string? city, string? country)
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
        ORDER BY n.id";

        AddParameter(command, "@name", string.IsNullOrWhiteSpace(name) ? null : name);
        AddParameter(command, "@city", string.IsNullOrWhiteSpace(city) ? null : city);
        AddParameter(command, "@country", string.IsNullOrWhiteSpace(country) ? null : country);

        using IDataReader reader = command.ExecuteReader();

        Dictionary<long, Neighborhood> neighborhoods = new();
        HashSet<long> addedStreets = new();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);

            if (!neighborhoods.ContainsKey(id))
            {
                neighborhoods[id] = new Neighborhood(
                    id,
                    reader["name"].ToString()!,
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

            if (!reader.IsDBNull(reader.GetOrdinal("street_id")))
            {
                long streetId = Convert.ToInt64(reader["street_id"]);

                if (!addedStreets.Contains(streetId))
                {
                    addedStreets.Add(streetId);

                    neighborhoods[id].AddStreet(new Street(
                        streetId,
                        id,
                        reader["street_name"].ToString()!,
                        Convert.ToInt32(reader["start_number"]),
                        Convert.ToInt32(reader["end_number"])
                    ));
                }
            }
        }

        List<Neighborhood> result = neighborhoods.Values.ToList();

        Dictionary<long, List<Image>> imagesByNeighborhood =
            _imageRepository.GetByEntities("neighborhood", result.Select(n => n.Id));

        foreach (Neighborhood neighborhood in result)
        {
            if (imagesByNeighborhood.TryGetValue(neighborhood.Id, out List<Image>? images))
            {
                foreach (Image image in images)
                {
                    neighborhood.AddImage(image);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(address))
        {
            string loweredAddress = address.ToLower().Trim();

            int? number = null;
            string streetPart = loweredAddress;

            string[] parts = loweredAddress.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts[^1], out int parsedNumber))
            {
                number = parsedNumber;
                streetPart = string.Join(" ", parts.Take(parts.Length - 1));
            }

            result = result.Where(n =>
                n.Streets.Any(s =>
                    s.StreetName.ToLower().Contains(streetPart) &&
                    (!number.HasValue || (number.Value >= s.StartNumber && number.Value <= s.EndNumber))
                )
            ).ToList();
        }

        return result;
    }

    public bool CheckAddressMatch(string fullAddress, long neighborhoodId)
    {
        if (string.IsNullOrWhiteSpace(fullAddress))
            return false;

        string Normalize(string s)
        {
            return s.Trim().ToLower()
                .Replace("š", "s")
                .Replace("đ", "d")
                .Replace("č", "c")
                .Replace("ć", "c")
                .Replace("ž", "z");
        }

        string normalized = Normalize(fullAddress);

        // NAĐI BROJ (robusnije)
        string numberStr = new string(normalized.Where(char.IsDigit).ToArray());

        if (!int.TryParse(numberStr, out int number))
            return false;

        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT street_name, start_number, end_number
        FROM neighborhood_streets
        WHERE neighborhood_id = @neighborhoodId";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            string dbStreet = Normalize(reader["street_name"].ToString()!);
            int start = Convert.ToInt32(reader["start_number"]);
            int end = Convert.ToInt32(reader["end_number"]);

            // 🔥 SAMO PROVERI DA LI ADRESA SADRŽI ULICU
            if (normalized.Contains(dbStreet))
            {
                if (number >= start && number <= end)
                    return true;
            }
        }

        return false;
    }

    private bool TryExtractStreetAndNumber(string address, out string street, out int number)
    {
        street = string.Empty;
        number = 0;

        if (string.IsNullOrWhiteSpace(address))
            return false;

        string cleaned = address.Trim();

        int numberStart = -1;
        for (int i = 0; i < cleaned.Length; i++)
        {
            if (char.IsDigit(cleaned[i]))
            {
                numberStart = i;
                break;
            }
        }

        if (numberStart == -1)
            return false;

        int numberEnd = numberStart;
        while (numberEnd < cleaned.Length && char.IsDigit(cleaned[numberEnd]))
        {
            numberEnd++;
        }

        string streetPart = cleaned.Substring(0, numberStart).Trim().Trim(',', '.', '-', '/');
        string numberPart = cleaned.Substring(numberStart, numberEnd - numberStart).Trim();

        if (string.IsNullOrWhiteSpace(streetPart))
            return false;

        if (!int.TryParse(numberPart, out number))
            return false;

        street = NormalizeStreetName(streetPart);
        return true;
    }

    private string NormalizeStreetName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string result = value.Trim().ToLowerInvariant();

        result = TransliterateSerbianCyrillicToLatin(result);

        result = result
            .Replace("š", "s")
            .Replace("đ", "d")
            .Replace("č", "c")
            .Replace("ć", "c")
            .Replace("ž", "z");

        result = result
            .Replace("ulica", " ")
            .Replace("ul.", " ")
            .Replace("ul ", " ");

        var filtered = new List<char>();
        foreach (char c in result)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            {
                filtered.Add(c);
            }
        }

        result = new string(filtered.ToArray());

        while (result.Contains("  "))
        {
            result = result.Replace("  ", " ");
        }

        return result.Trim();
    }

    private string TransliterateSerbianCyrillicToLatin(string input)
    {
        var map = new Dictionary<char, string>
        {
            ['а'] = "a",
            ['б'] = "b",
            ['в'] = "v",
            ['г'] = "g",
            ['д'] = "d",
            ['ђ'] = "d",
            ['е'] = "e",
            ['ж'] = "z",
            ['з'] = "z",
            ['и'] = "i",
            ['ј'] = "j",
            ['к'] = "k",
            ['л'] = "l",
            ['љ'] = "lj",
            ['м'] = "m",
            ['н'] = "n",
            ['њ'] = "nj",
            ['о'] = "o",
            ['п'] = "p",
            ['р'] = "r",
            ['с'] = "s",
            ['т'] = "t",
            ['ћ'] = "c",
            ['у'] = "u",
            ['ф'] = "f",
            ['х'] = "h",
            ['ц'] = "c",
            ['ч'] = "c",
            ['џ'] = "dz",
            ['ш'] = "s"
        };

        var result = new StringBuilder();

        foreach (char c in input)
        {
            if (map.TryGetValue(c, out string? latin))
                result.Append(latin);
            else
                result.Append(c);
        }

        return result.ToString();
    }

    public void CreateMembership(long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO neighborhood_memberships (citizen_id, neighborhood_id, joined_at)
        VALUES (@citizenId, @neighborhoodId, CURRENT_DATE)";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);

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

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);

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

        AddParameter(command, "@citizenId", citizenId);

        if (!string.IsNullOrEmpty(statusFilter))
        {
            command.CommandText += " AND r.status::text = @status";
            AddParameter(command, "@status", statusFilter);
        }

        command.CommandText += " ORDER BY r.created_at DESC";

        using IDataReader reader = command.ExecuteReader();

        var requests = new List<NeighborhoodAccessRequest>();

        while (reader.Read())
        {
            User citizen = new User(
                Convert.ToInt64(reader["citizen_id"]),
                reader["username"].ToString()!,
                reader["password"].ToString()!,
                reader["citizen_name"].ToString()!,
                reader["citizen_surname"].ToString()!,
                ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
                UserMapper.ParseRole(reader["role"].ToString()!),
                reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
            );

            Neighborhood neighborhood = new Neighborhood(
                Convert.ToInt64(reader["n_id"]),
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

            requests.Add(new NeighborhoodAccessRequest(
                Convert.ToInt64(reader["id"]),
                citizen,
                neighborhood,
                Convert.ToDateTime(reader["created_at"]),
                ParseRequestStatus(reader["status"].ToString()!),
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

        AddParameter(command, "@id", requestId);

        command.ExecuteNonQuery();
    }

    public void AddImage(long neighborhoodId, string imagePath)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO images (entity, entity_id, path)
        VALUES ('neighborhood', @neighborhoodId, @path)";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@path", imagePath);

        command.ExecuteNonQuery();
    }

    public List<Image> GetImages(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, path FROM images
        WHERE entity = 'neighborhood' AND entity_id = @neighborhoodId";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();

        var images = new List<Image>();
        while (reader.Read())
        {
            images.Add(new Image(
                Convert.ToInt64(reader["id"]),
                reader["path"].ToString()!
            ));
        }

        return images;
    }

    private static RequestStatus ParseRequestStatus(string status)
    {
        return status.Trim().ToLower() switch
        {
            "pending approval" => RequestStatus.PendingApproval,
            "pendingapproval" => RequestStatus.PendingApproval,
            "pending_approval" => RequestStatus.PendingApproval,
            "approved" => RequestStatus.Approved,
            "rejected" => RequestStatus.Rejected,
            _ => throw new ArgumentException($"Unknown status: {status}")
        };
    }
}
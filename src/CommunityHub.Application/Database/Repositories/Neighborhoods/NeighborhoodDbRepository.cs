using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.Domain.Shared;
using System.Data;
using System.Linq;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class NeighborhoodDbRepository : BaseDbRepository, INeighborhoodRepository
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
        return ReadNeighborhoodsWithStreets(reader);
    }

    public List<Neighborhood> SearchForCitizen(string? name, string? address, string? city, string? country)
    {
        List<Neighborhood> result = FetchNeighborhoodsFromDb(name, city, country);
        AttachImagesToNeighborhoods(result);
        if (!string.IsNullOrWhiteSpace(address))
            result = FilterByAddress(result, address);
        return result;
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
        var neighborhoods = ReadNeighborhoodsWithStreets(reader);

        foreach (var neighborhood in neighborhoods)
        {
            var images = GetImages(neighborhood.Id);
            foreach (var image in images)
                neighborhood.AddImage(image);
        }

        return neighborhoods;
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
            images.Add(new Image(Convert.ToInt64(reader["id"]), reader["path"].ToString()!));

        return images;
    }

    private List<Neighborhood> FetchNeighborhoodsFromDb(string? name, string? city, string? country)
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
        return ReadNeighborhoodsWithStreets(reader);
    }

    private List<Neighborhood> ReadNeighborhoodsWithStreets(IDataReader reader)
    {
        Dictionary<long, Neighborhood> neighborhoods = new();
        HashSet<long> addedStreets = new();

        while (reader.Read())
        {
            long neighborhoodId = Convert.ToInt64(reader["id"]);

            if (!neighborhoods.ContainsKey(neighborhoodId))
                neighborhoods[neighborhoodId] = MapNeighborhood(reader);

            if (!reader.IsDBNull(reader.GetOrdinal("street_id")))
                TryAddStreet(reader, neighborhoodId, neighborhoods, addedStreets);
        }

        return neighborhoods.Values.ToList();
    }

    private Neighborhood MapNeighborhood(IDataReader reader)
    {
        return new Neighborhood(
            Convert.ToInt64(reader["id"]),
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

    private void TryAddStreet(IDataReader reader, long neighborhoodId, Dictionary<long, Neighborhood> neighborhoods, HashSet<long> addedStreets)
    {
        long streetId = Convert.ToInt64(reader["street_id"]);
        if (addedStreets.Contains(streetId))
            return;

        addedStreets.Add(streetId);
        neighborhoods[neighborhoodId].AddStreet(new Street(
            streetId,
            neighborhoodId,
            reader["street_name"].ToString()!,
            Convert.ToInt32(reader["start_number"]),
            Convert.ToInt32(reader["end_number"])
        ));
    }

    private void AttachImagesToNeighborhoods(List<Neighborhood> neighborhoods)
    {
        Dictionary<long, List<Image>> imagesByNeighborhood =
            _imageRepository.GetByEntities("neighborhood", neighborhoods.Select(n => n.Id));

        foreach (Neighborhood neighborhood in neighborhoods)
        {
            if (imagesByNeighborhood.TryGetValue(neighborhood.Id, out List<Image>? images))
                foreach (Image image in images)
                    neighborhood.AddImage(image);
        }
    }

    private List<Neighborhood> FilterByAddress(List<Neighborhood> neighborhoods, string address)
    {
        string lowered = address.ToLower().Trim();
        string[] parts = lowered.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        int? number = null;
        string streetPart = lowered;

        if (parts.Length > 1 && int.TryParse(parts[^1], out int parsedNumber))
        {
            number = parsedNumber;
            streetPart = string.Join(" ", parts.Take(parts.Length - 1));
        }

        return neighborhoods.Where(n => MatchesAddressFilter(n, streetPart, number)).ToList();
    }

    private bool MatchesAddressFilter(Neighborhood neighborhood, string streetPart, int? number)
    {
        return neighborhood.Streets.Any(s =>
            s.StreetName.ToLower().Contains(streetPart) &&
            (!number.HasValue || (number.Value >= s.StartNumber && number.Value <= s.EndNumber))
        );
    }

    public Neighborhood? GetById(long neighborhoodId)
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
        WHERE n.id = @id";

        AddParameter(command, "@id", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        var neighborhoods = ReadNeighborhoodsWithStreets(reader);
        return neighborhoods.FirstOrDefault();
    }

    public string? GetNameById(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM neighborhoods WHERE id = @id";
        AddParameter(command, "@id", neighborhoodId);
        object? result = command.ExecuteScalar();
        return result == null || result == DBNull.Value ? null : result.ToString();
    }
    
}
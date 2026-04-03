using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingDbRepository
{
    public List<Building> GetAll()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT b.id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   f.id AS floor_id, f.floor_number,
                   u.id AS unit_id, u.unit_number,
                   i.id AS image_id, i.path AS image_path
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
            LEFT JOIN images i ON i.resource_id = b.id AND i.resource = 'building'
            ORDER BY b.id, f.floor_number, u.unit_number";

        using IDataReader reader = command.ExecuteReader();
        return ReadBuildings(reader);
    }

    public List<Building> Search(string? street, string? neighborhood, string? city, string? country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT b.id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   f.id AS floor_id, f.floor_number,
                   u.id AS unit_id, u.unit_number,
                   i.id AS image_id, i.path AS image_path
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
            LEFT JOIN images i ON i.resource_id = b.id AND i.resource = 'building'
            WHERE (@street IS NULL OR b.street ILIKE '%' || @street || '%'
                   OR b.street_number ILIKE '%' || @street || '%')
              AND (@neighborhood IS NULL OR b.neighborhood ILIKE '%' || @neighborhood || '%')
              AND (@city IS NULL OR c.name ILIKE '%' || @city || '%')
              AND (@country IS NULL OR co.name ILIKE '%' || @country || '%')
            ORDER BY b.id, f.floor_number, u.unit_number";

        AddParameter(command, "@street", street);
        AddParameter(command, "@neighborhood", neighborhood);
        AddParameter(command, "@city", city);
        AddParameter(command, "@country", country);

        using IDataReader reader = command.ExecuteReader();
        return ReadBuildings(reader);
    }

    private List<Building> ReadBuildings(IDataReader reader)
    {
        Dictionary<long, Building> buildings = new Dictionary<long, Building>();
        Dictionary<long, Floor> floors = new Dictionary<long, Floor>();
        HashSet<long> addedImages = new HashSet<long>();
        HashSet<long> addedUnits = new HashSet<long>();

        while (reader.Read())
        {
            long buildingId = Convert.ToInt64(reader["id"]);

            if (!buildings.ContainsKey(buildingId))
                buildings[buildingId] = MapBuilding(reader);

            AddFloorIfMissing(reader, buildings, floors);
            AddUnitIfMissing(reader, floors, addedUnits);
            AddImageIfMissing(reader, buildings, addedImages);
        }

        return buildings.Values.ToList();
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
            Convert.ToInt64(reader["id"]),
            reader["street"].ToString(),
            reader["street_number"].ToString(),
            reader["neighborhood"].ToString(),
            city,
            Convert.ToInt32(reader["number_of_floors"])
        );
    }

    private void AddFloorIfMissing(IDataReader reader, Dictionary<long, Building> buildings, Dictionary<long, Floor> floors)
    {
        if (reader.IsDBNull(reader.GetOrdinal("floor_id"))) return;

        long floorId = Convert.ToInt64(reader["floor_id"]);
        long buildingId = Convert.ToInt64(reader["id"]);

        if (floors.ContainsKey(floorId)) return;

        Floor floor = new Floor(floorId, buildings[buildingId], Convert.ToInt32(reader["floor_number"]));
        floors[floorId] = floor;
        buildings[buildingId].AddFloor(floor);
    }

    private void AddUnitIfMissing(IDataReader reader, Dictionary<long, Floor> floors, HashSet<long> addedUnits)
    {
        if (reader.IsDBNull(reader.GetOrdinal("floor_id"))) return;
        if (reader.IsDBNull(reader.GetOrdinal("unit_id"))) return;

        long unitId = Convert.ToInt64(reader["unit_id"]);
        if (addedUnits.Contains(unitId)) return;

        long floorId = Convert.ToInt64(reader["floor_id"]);
        Unit unit = new Unit(unitId, floors[floorId], reader["unit_number"].ToString());
        floors[floorId].AddUnit(unit);
        addedUnits.Add(unitId);
    }

    private void AddImageIfMissing(IDataReader reader, Dictionary<long, Building> buildings, HashSet<long> addedImages)
    {
        if (reader.IsDBNull(reader.GetOrdinal("image_id"))) return;

        long imageId = Convert.ToInt64(reader["image_id"]);
        if (addedImages.Contains(imageId)) return;

        long buildingId = Convert.ToInt64(reader["id"]);
        buildings[buildingId].AddImage(new AppImage(imageId, reader["image_path"].ToString()));
        addedImages.Add(imageId);
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        command.Parameters.Add(param);
    }

    public List<string> GetOccupiedUnits(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT unit_number FROM building_memberships
        WHERE building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        List<string> occupiedUnits = new List<string>();
        while (reader.Read())
            occupiedUnits.Add(reader["unit_number"].ToString());

        return occupiedUnits;
    }
}
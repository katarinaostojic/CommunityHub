using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingDbRepository : BaseDbRepository
{
    private readonly ImageDbRepository _imageRepository;

    public BuildingDbRepository()
    {
        _imageRepository = new ImageDbRepository();
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
                   u.id AS unit_id, u.unit_number
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
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
        List<Building> buildings = ReadBuildings(reader);
        AttachImages(buildings);
        return buildings;
    }

    public Building? GetById(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT b.id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   f.id AS floor_id, f.floor_number,
                   u.id AS unit_id, u.unit_number
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
            WHERE b.id = @buildingId
            ORDER BY f.floor_number, u.unit_number";

        AddParameter(command, "@buildingId", buildingId);

        List<Building> buildings;
        using (IDataReader reader = command.ExecuteReader())
            buildings = ReadBuildings(reader);

        if (buildings.Count == 0) return null;

        Building building = buildings[0];
        PopulateBuildingDetails(building, connection, buildingId);
        return building;
    }

    private void PopulateBuildingDetails(Building building, IDbConnection connection, long buildingId)
    {
        foreach (Image image in _imageRepository.GetByEntity("building", buildingId))
            building.AddImage(image);

        foreach (BuildingMembership membership in GetMembershipsByBuilding(connection, buildingId))
            building.AddMembership(membership);

        foreach (BuildingAccessRequest request in GetAccessRequestsByBuilding(connection, buildingId))
            building.AddAccessRequest(request);
    }

    private List<BuildingMembership> GetMembershipsByBuilding(IDbConnection connection, long buildingId)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT bm.id, bm.unit_number, bm.floor_number, bm.approved_at,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_memberships bm
            JOIN users u ON bm.user_id = u.id
            WHERE bm.building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        List<BuildingMembership> memberships = new List<BuildingMembership>();
        while (reader.Read())
            memberships.Add(BuildingMembershipMapper.MapWithoutBuilding(reader));
        return memberships;
    }

    private List<BuildingAccessRequest> GetAccessRequestsByBuilding(IDbConnection connection, long buildingId)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT r.id, r.unit_number, r.created_at, r.status, r.rejection_reason,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_access_requests r
            JOIN users u ON r.user_id = u.id
            WHERE r.building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        List<BuildingAccessRequest> requests = new List<BuildingAccessRequest>();
        while (reader.Read())
        {
            requests.Add(BuildingAccessRequestMapper.MapWithoutBuilding(reader));
        }
        return requests;
    }

    public List<Building> GetAllByManager(long managerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT b.id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   f.id AS floor_id, f.floor_number,
                   u.id AS unit_id, u.unit_number
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
            WHERE b.manager_id = @managerId
            ORDER BY b.id, f.floor_number, u.unit_number";

        AddParameter(command, "@managerId", managerId);

        using IDataReader reader = command.ExecuteReader();
        List<Building> buildings = ReadBuildings(reader);
        AttachImages(buildings);
        return buildings;
    }

    public long CreateBuilding(string street, string streetNumber, string neighborhood, long cityId, int numberOfFloors, long managerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO buildings (street, street_number, neighborhood, city_id, number_of_floors, manager_id)
            VALUES (@street, @streetNumber, @neighborhood, @cityId, @numberOfFloors, @managerId)
            RETURNING id";

        AddParameter(command, "@street", street);
        AddParameter(command, "@streetNumber", streetNumber);
        AddParameter(command, "@neighborhood", neighborhood);
        AddParameter(command, "@cityId", cityId);
        AddParameter(command, "@numberOfFloors", numberOfFloors);
        AddParameter(command, "@managerId", managerId);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public long CreateFloorReturningId(long buildingId, int floorNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO floors (building_id, floor_number)
            VALUES (@buildingId, @floorNumber)
            RETURNING id";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@floorNumber", floorNumber);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void CreateUnit(long floorId, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO units (floor_id, unit_number)
            VALUES (@floorId, @unitNumber)";

        AddParameter(command, "@floorId", floorId);
        AddParameter(command, "@unitNumber", unitNumber);

        command.ExecuteNonQuery();
    }

    private List<Building> ReadBuildings(IDataReader reader)
    {
        Dictionary<long, Building> buildings = new Dictionary<long, Building>();
        Dictionary<long, Floor> floors = new Dictionary<long, Floor>();
        HashSet<long> addedUnits = new HashSet<long>();

        while (reader.Read())
        {
            long buildingId = Convert.ToInt64(reader["id"]);

            if (!buildings.ContainsKey(buildingId))
                buildings[buildingId] = BuildingMapper.Map(reader);

            AddFloorIfMissing(reader, buildings, floors);
            AddUnitIfMissing(reader, floors, addedUnits);
        }

        return buildings.Values.ToList();
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
        Unit unit = new Unit(unitId, floors[floorId], reader["unit_number"].ToString()!);
        floors[floorId].AddUnit(unit);
        addedUnits.Add(unitId);
    }

    private void AttachImages(List<Building> buildings)
    {
        if (buildings.Count == 0) return;

        Dictionary<long, List<Image>> imageMap =
            _imageRepository.GetByEntities("building", buildings.Select(b => b.Id));

        foreach (Building building in buildings)
            foreach (Image image in imageMap[building.Id])
                building.AddImage(image);
    }

    public bool BuildingExists(string street, string streetNumber, long cityId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM buildings
        WHERE LOWER(street) = LOWER(@street)
          AND LOWER(street_number) = LOWER(@streetNumber)
          AND city_id = @cityId";

        AddParameter(command, "@street", street);
        AddParameter(command, "@streetNumber", streetNumber);
        AddParameter(command, "@cityId", cityId);

        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }
}
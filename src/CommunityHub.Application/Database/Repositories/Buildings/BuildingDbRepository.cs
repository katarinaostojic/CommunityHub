using CommunityHub.Application.Database.Readers.Buildings;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings;

public class BuildingDbRepository : BaseDbRepository, IBuildingRepository
{
    private readonly BuildingDetailsDbRepository _detailsRepository;

    public BuildingDbRepository(BuildingDetailsDbRepository detailsRepository)
    {
        _detailsRepository = detailsRepository;
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
        List<Building> buildings = BuildingReader.ReadBuildings(reader);

        _detailsRepository.AttachImages(buildings);
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
        {
            buildings = BuildingReader.ReadBuildings(reader);
        }

        if (buildings.Count == 0)
        {
            return null;
        }

        Building building = buildings[0];
        _detailsRepository.PopulateBuildingDetails(building, connection, buildingId);

        return building;
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
        List<Building> buildings = BuildingReader.ReadBuildings(reader);

        _detailsRepository.AttachImages(buildings);
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

    public long CreateFloor(long buildingId, int floorNumber)
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
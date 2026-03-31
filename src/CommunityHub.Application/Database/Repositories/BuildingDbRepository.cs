using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingDbRepository
{
    public List<Building> GetAll()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT b.id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   f.id AS floor_id, f.floor_number,
                   u.id AS unit_id, u.unit_number,
                   bi.image_path
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
            LEFT JOIN building_images bi ON bi.building_id = b.id
            ORDER BY b.id, f.floor_number, u.unit_number";

        using IDataReader reader = command.ExecuteReader();

        Dictionary<long, Building> buildings = new Dictionary<long, Building>();
        Dictionary<long, Floor> floors = new Dictionary<long, Floor>();
        HashSet<string> addedImages = new HashSet<string>();

        while (reader.Read())
        {
            long buildingId = Convert.ToInt64(reader["id"]);

            if (!buildings.ContainsKey(buildingId))
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

                buildings[buildingId] = new Building(
                    buildingId,
                    reader["street"].ToString(),
                    reader["street_number"].ToString(),
                    reader["neighborhood"].ToString(),
                    city,
                    Convert.ToInt32(reader["number_of_floors"])
                );
            }

            if (!reader.IsDBNull(reader.GetOrdinal("floor_id")))
            {
                long floorId = Convert.ToInt64(reader["floor_id"]);

                if (!floors.ContainsKey(floorId))
                {
                    Floor floor = new Floor(
                        floorId,
                        buildings[buildingId],
                        Convert.ToInt32(reader["floor_number"])
                    );
                    floors[floorId] = floor;
                    buildings[buildingId].AddFloor(floor);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("unit_id")))
                {
                    long unitId = Convert.ToInt64(reader["unit_id"]);
                    Unit unit = new Unit(
                        unitId,
                        floors[floorId],
                        reader["unit_number"].ToString()
                    );
                    floors[floorId].AddUnit(unit);
                }
            }

            if (!reader.IsDBNull(reader.GetOrdinal("image_path")))
            {
                string imagePath = reader["image_path"].ToString();
                string imageKey = $"{buildingId}_{imagePath}";
                if (!addedImages.Contains(imageKey))
                {
                    buildings[buildingId].AddImagePath(imagePath);
                    addedImages.Add(imageKey);
                }
            }
        }

        return buildings.Values.ToList();
    }

    public List<Building> Search(string? street, string? neighborhood, string? city, string? country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT b.id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   f.id AS floor_id, f.floor_number,
                   u.id AS unit_id, u.unit_number,
                   bi.image_path
            FROM buildings b
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            LEFT JOIN floors f ON f.building_id = b.id
            LEFT JOIN units u ON u.floor_id = f.id
            LEFT JOIN building_images bi ON bi.building_id = b.id
            WHERE (@street IS NULL OR b.street ILIKE '%' || @street || '%'
                   OR b.street_number ILIKE '%' || @street || '%')
              AND (@neighborhood IS NULL OR b.neighborhood ILIKE '%' || @neighborhood || '%')
              AND (@city IS NULL OR c.name ILIKE '%' || @city || '%')
              AND (@country IS NULL OR co.name ILIKE '%' || @country || '%')
            ORDER BY b.id, f.floor_number, u.unit_number";

        IDbDataParameter streetParam = command.CreateParameter();
        streetParam.ParameterName = "@street";
        streetParam.Value = (object?)street ?? DBNull.Value;
        streetParam.DbType = DbType.String;
        command.Parameters.Add(streetParam);

        IDbDataParameter neighborhoodParam = command.CreateParameter();
        neighborhoodParam.ParameterName = "@neighborhood";
        neighborhoodParam.Value = (object?)neighborhood ?? DBNull.Value;
        neighborhoodParam.DbType = DbType.String;
        command.Parameters.Add(neighborhoodParam);

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

        Dictionary<long, Building> buildings = new Dictionary<long, Building>();
        Dictionary<long, Floor> floors = new Dictionary<long, Floor>();
        HashSet<string> addedImages = new HashSet<string>();

        while (reader.Read())
        {
            long buildingId = Convert.ToInt64(reader["id"]);

            if (!buildings.ContainsKey(buildingId))
            {
                Country country_ = new Country(
                    Convert.ToInt64(reader["country_id"]),
                    reader["country_name"].ToString(),
                    reader["country_code"].ToString()
                );

                City city_ = new City(
                    Convert.ToInt64(reader["city_id"]),
                    reader["city_name"].ToString(),
                    country_
                );

                buildings[buildingId] = new Building(
                    buildingId,
                    reader["street"].ToString(),
                    reader["street_number"].ToString(),
                    reader["neighborhood"].ToString(),
                    city_,
                    Convert.ToInt32(reader["number_of_floors"])
                );
            }

            if (!reader.IsDBNull(reader.GetOrdinal("floor_id")))
            {
                long floorId = Convert.ToInt64(reader["floor_id"]);

                if (!floors.ContainsKey(floorId))
                {
                    Floor floor = new Floor(
                        floorId,
                        buildings[buildingId],
                        Convert.ToInt32(reader["floor_number"])
                    );
                    floors[floorId] = floor;
                    buildings[buildingId].AddFloor(floor);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("unit_id")))
                {
                    long unitId = Convert.ToInt64(reader["unit_id"]);
                    Unit unit = new Unit(
                        unitId,
                        floors[floorId],
                        reader["unit_number"].ToString()
                    );
                    floors[floorId].AddUnit(unit);
                }
            }

            if (!reader.IsDBNull(reader.GetOrdinal("image_path")))
            {
                string imagePath = reader["image_path"].ToString();
                string imageKey = $"{buildingId}_{imagePath}";
                if (!addedImages.Contains(imageKey))
                {
                    buildings[buildingId].AddImagePath(imagePath);
                    addedImages.Add(imageKey);
                }
            }
        }

        return buildings.Values.ToList();
    }
}
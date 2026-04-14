using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Mappers;

public static class BuildingMapper
{
    public static Building Map(IDataReader reader, string idColumn = "id")
    {
        return new Building(
            Convert.ToInt64(reader[idColumn]),
            reader["street"].ToString()!,
            reader["street_number"].ToString()!,
            reader["neighborhood"].ToString()!,
            MapCity(reader),
            Convert.ToInt32(reader["number_of_floors"])
        );
    }

    private static City MapCity(IDataReader reader)
    {
        Country country = new Country(
            Convert.ToInt64(reader["country_id"]),
            reader["country_name"].ToString()!,
            reader["country_code"].ToString()!
        );

        return new City(
            Convert.ToInt64(reader["city_id"]),
            reader["city_name"].ToString()!,
            country
        );
    }
}
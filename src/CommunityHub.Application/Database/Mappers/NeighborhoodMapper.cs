using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Mappers;

public static class NeighborhoodMapper
{
    public static Neighborhood Map(IDataReader reader)
    {
        return new Neighborhood(
            Convert.ToInt64(reader["id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            MapLocation(reader),
            Convert.ToDecimal(reader["budget"]),
            Convert.ToInt64(reader["coordinator_id"])
        );
    }

    public static Neighborhood MapFromJoin(IDataReader reader)
    {
        return new Neighborhood(
            Convert.ToInt64(reader["neighborhood_id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            MapLocation(reader),
            Convert.ToDecimal(reader["budget"]),
            Convert.ToInt64(reader["coordinator_id"])
        );
    }

    private static Location MapLocation(IDataReader reader)
    {
        return new Location(
            Convert.ToInt64(reader["location_id"]),
            reader["city_name"].ToString()!,
            reader["country_name"].ToString()!
        );
    }
}
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

    public static Neighborhood MapFromRequest(IDataReader reader)
    {
        return new Neighborhood(
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
    }

    public static User MapRequestCitizen(IDataReader reader)
    {
        return new User(
            Convert.ToInt64(reader["citizen_id"]),
            reader["username"].ToString()!,
            reader["password"].ToString()!,
            reader["citizen_name"].ToString()!,
            reader["citizen_surname"].ToString()!,
            ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
            UserMapper.ParseRole(reader["role"].ToString()!),
            reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
        );
    }

    public static Location MapLocation(IDataReader reader)
    {
        return new Location(
            Convert.ToInt64(reader["city_id"]),
            reader["city_name"].ToString()!,
            reader["country_name"].ToString()!
        );
    }
}
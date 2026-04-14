using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class NeighborhoodMembershipDbRepository : BaseDbRepository
{
    public List<NeighborhoodMembership> GetByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT nm.id, nm.citizen_id, nm.neighborhood_id, nm.joined_at,
               u.username, u.password, u.name, u.surname, u.birthday, u.role, u.address
        FROM neighborhood_memberships nm
        JOIN users u ON nm.citizen_id = u.id
        WHERE nm.neighborhood_id = @neighborhoodId
        ORDER BY nm.joined_at DESC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();

        var memberships = new List<NeighborhoodMembership>();
        while (reader.Read())
        {
            User citizen = new User(
                Convert.ToInt64(reader["citizen_id"]),
                reader["username"].ToString()!,
                reader["password"].ToString()!,
                reader["name"].ToString()!,
                reader["surname"].ToString()!,
                ((DateOnly)reader["birthday"]).ToDateTime(TimeOnly.MinValue),
                UserMapper.ParseRole(reader["role"].ToString()!),
                reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader["address"].ToString()
            );

            memberships.Add(new NeighborhoodMembership(
                Convert.ToInt64(reader["id"]),
                citizen,
                Convert.ToInt64(reader["neighborhood_id"]),
                Convert.ToDateTime(reader["joined_at"])
            ));
        }

        return memberships;
    }
}
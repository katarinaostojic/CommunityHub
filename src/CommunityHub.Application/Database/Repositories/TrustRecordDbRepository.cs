using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class TrustRecordDbRepository : BaseDbRepository
{
    public List<TrustRecord> GetByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT nm.citizen_id, nm.joined_at
        FROM neighborhood_memberships nm
        WHERE nm.neighborhood_id = @neighborhoodId";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        var records = new List<TrustRecord>();
        while (reader.Read())
        {
            records.Add(new TrustRecord(
                Convert.ToInt64(reader["citizen_id"]),
                0,
                0,
                (DateOnly)reader["joined_at"]
            ));
        }
        return records;
    }
     public Dictionary<TrustLevel, int> GetTrustLevelCounts(long neighborhoodId)
    {
        var records = GetByNeighborhood(neighborhoodId);
        var counts = new Dictionary<TrustLevel, int>
        {
            [TrustLevel.New] = 0,
            [TrustLevel.Inactive] = 0,
            [TrustLevel.Active] = 0,
            [TrustLevel.Distinguished] = 0,
            [TrustLevel.Trusted] = 0
        };

        foreach (var record in records)
            counts[record.GetLevel()]++;

        return counts;
    }
}
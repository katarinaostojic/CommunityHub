using System.Data;
using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

namespace CommunityHub.Application.Database.Repositories.Shared;

public class TrustRecordDbRepository : BaseDbRepository, ITrustRecordRepository
{
    public TrustRecord GetByCitizen(long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                nm.citizen_id,
                nm.joined_at,
                COUNT(DISTINCT CASE WHEN e.organizer_id = nm.citizen_id AND e.status = 'finished' THEN e.id END) AS events_organized,
                COUNT(DISTINCT CASE WHEN er.citizen_id = nm.citizen_id AND er.attended = true THEN er.id END) AS events_volunteered
            FROM neighborhood_memberships nm
            LEFT JOIN events e ON e.neighborhood_id = nm.neighborhood_id
            LEFT JOIN event_registrations er ON er.event_id = e.id AND er.citizen_id = nm.citizen_id
            WHERE nm.citizen_id = @citizenId AND nm.neighborhood_id = @neighborhoodId
            GROUP BY nm.citizen_id, nm.joined_at";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
            return MapTrustRecord(reader);

        return new TrustRecord(citizenId, 0, 0, DateOnly.FromDateTime(DateTime.Now));
    }

    public List<TrustRecord> GetByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                nm.citizen_id,
                nm.joined_at,
                COUNT(DISTINCT CASE WHEN e.organizer_id = nm.citizen_id AND e.status = 'finished' THEN e.id END) AS events_organized,
                COUNT(DISTINCT CASE WHEN er.citizen_id = nm.citizen_id AND er.attended = true THEN er.id END) AS events_volunteered
            FROM neighborhood_memberships nm
            LEFT JOIN events e ON e.neighborhood_id = nm.neighborhood_id
            LEFT JOIN event_registrations er ON er.event_id = e.id AND er.citizen_id = nm.citizen_id
            WHERE nm.neighborhood_id = @neighborhoodId
            GROUP BY nm.citizen_id, nm.joined_at";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        var records = new List<TrustRecord>();
        while (reader.Read())
            records.Add(MapTrustRecord(reader));

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

    private TrustRecord MapTrustRecord(IDataReader reader)
    {
        return new TrustRecord(
            Convert.ToInt64(reader["citizen_id"]),
            Convert.ToInt32(reader["events_organized"]),
            Convert.ToInt32(reader["events_volunteered"]),
            (DateOnly)reader["joined_at"]
        );
    }
}
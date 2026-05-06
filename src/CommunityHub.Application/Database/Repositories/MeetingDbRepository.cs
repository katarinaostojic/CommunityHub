using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class MeetingDbRepository : BaseDbRepository
{
    public long Create(Meeting meeting)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO meetings (neighborhood_id, theme, meeting_time, date_range_start, date_range_end, status)
            VALUES (@neighborhoodId, @theme, @meetingTime, @dateRangeStart, @dateRangeEnd, 'in_preparation')
            RETURNING id";

        AddParameter(command, "@neighborhoodId", meeting.NeighborhoodId);
        AddParameter(command, "@theme", meeting.Theme == MeetingTheme.Welcome ? "welcome" : "motivation");
        AddParameter(command, "@meetingTime", meeting.MeetingTime.ToTimeSpan());
        AddParameter(command, "@dateRangeStart", DateTime.SpecifyKind(meeting.DateRangeStart.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));
        AddParameter(command, "@dateRangeEnd", DateTime.SpecifyKind(meeting.DateRangeEnd.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public List<Meeting> GetByCoordinator(long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT m.id, m.neighborhood_id, m.theme, m.meeting_time,
                   m.date_range_start, m.date_range_end, m.status, m.scheduled_date
            FROM meetings m
            JOIN neighborhoods n ON m.neighborhood_id = n.id
            WHERE n.coordinator_id = @coordinatorId
            ORDER BY m.date_range_start";

        AddParameter(command, "@coordinatorId", coordinatorId);

        using IDataReader reader = command.ExecuteReader();
        var meetings = new List<Meeting>();
        while (reader.Read())
            meetings.Add(MapMeeting(reader));
        return meetings;
    }

    public void UpdateStatus(long meetingId, MeetingStatus status, DateOnly? scheduledDate)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE meetings 
            SET status = @status, scheduled_date = @scheduledDate
            WHERE id = @id";

        AddParameter(command, "@id", meetingId);
        AddParameter(command, "@status", ParseStatusToString(status));
        IDbDataParameter scheduledDateParam = command.CreateParameter();
        scheduledDateParam.ParameterName = "@scheduledDate";
        scheduledDateParam.Value = scheduledDate.HasValue
            ? (object)scheduledDate.Value.ToDateTime(TimeOnly.MinValue)
            : DBNull.Value;
        scheduledDateParam.DbType = DbType.DateTime;
        command.Parameters.Add(scheduledDateParam);
        command.ExecuteNonQuery();
    }

    public void AddVote(MeetingVote vote)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO meeting_votes (meeting_id, citizen_id, voted_date)
            VALUES (@meetingId, @citizenId, @votedDate)";

        AddParameter(command, "@meetingId", vote.MeetingId);
        AddParameter(command, "@citizenId", vote.CitizenId);
        AddParameter(command, "@votedDate", vote.VotedDate.ToDateTime(TimeOnly.MinValue));

        command.ExecuteNonQuery();
    }

    public Dictionary<DateOnly, int> GetVoteCounts(long meetingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT voted_date, COUNT(*) as vote_count
            FROM meeting_votes
            WHERE meeting_id = @meetingId
            GROUP BY voted_date
            ORDER BY vote_count DESC";

        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        var result = new Dictionary<DateOnly, int>();
        while (reader.Read())
        {
            DateOnly date = DateOnly.FromDateTime(Convert.ToDateTime(reader["voted_date"]));
            int count = Convert.ToInt32(reader["vote_count"]);
            result[date] = count;
        }
        return result;
    }

    private Meeting MapMeeting(IDataReader reader)
    {
        DateOnly? scheduledDate = reader.IsDBNull(reader.GetOrdinal("scheduled_date"))
            ? null
            : (DateOnly)reader["scheduled_date"];

        return new Meeting(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            ParseTheme(reader["theme"].ToString()!),
            (TimeOnly)reader["meeting_time"],
            (DateOnly)reader["date_range_start"],
            (DateOnly)reader["date_range_end"],
            ParseStatus(reader["status"].ToString()!),
            scheduledDate
        );
    }

    private MeetingTheme ParseTheme(string theme) => theme.ToLower() switch
    {
        "welcome" => MeetingTheme.Welcome,
        "motivation" => MeetingTheme.Motivation,
        _ => throw new ArgumentException($"Unknown theme: {theme}")
    };

    private MeetingStatus ParseStatus(string status) => status.ToLower() switch
    {
        "in_preparation" => MeetingStatus.InPreparation,
        "scheduled" => MeetingStatus.Scheduled,
        "cancelled" => MeetingStatus.Cancelled,
        _ => throw new ArgumentException($"Unknown status: {status}")
    };

    private string ParseStatusToString(MeetingStatus status) => status switch
    {
        MeetingStatus.InPreparation => "in_preparation",
        MeetingStatus.Scheduled => "scheduled",
        MeetingStatus.Cancelled => "cancelled",
        _ => throw new ArgumentException($"Unknown status: {status}")
    };
}
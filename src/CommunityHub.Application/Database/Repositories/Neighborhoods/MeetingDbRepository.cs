using CommunityHub.Application.Domain.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class MeetingDbRepository : BaseDbRepository
{
    public long Create(Meeting meeting)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO meetings (neighborhood_id, theme, custom_theme_name, meeting_time, date_range_start, date_range_end, status)
        VALUES (@neighborhoodId, @theme, @customThemeName, @meetingTime, @date_range_start, @date_range_end, 'in_preparation')
        RETURNING id";

        AddParameter(command, "@neighborhoodId", meeting.NeighborhoodId);
        AddParameter(command, "@theme", meeting.Theme == MeetingTheme.Welcome ? "welcome" : meeting.Theme == MeetingTheme.Motivation ? "motivation" : "custom");
        if (meeting.CustomThemeName != null)
            AddParameter(command, "@customThemeName", meeting.CustomThemeName);
        else
        {
            IDbDataParameter customThemeParam = command.CreateParameter();
            customThemeParam.ParameterName = "@customThemeName";
            customThemeParam.Value = DBNull.Value;
            command.Parameters.Add(customThemeParam);
        }
        AddParameter(command, "@meetingTime", meeting.MeetingTime.ToTimeSpan());
        AddParameter(command, "@date_range_start", DateTime.SpecifyKind(meeting.DateRangeStart.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));
        AddParameter(command, "@date_range_end", DateTime.SpecifyKind(meeting.DateRangeEnd.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public List<Meeting> GetByCoordinator(long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT m.id, m.neighborhood_id, m.theme, m.custom_theme_name, m.meeting_time,
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
            ? (object)DateTime.SpecifyKind(scheduledDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
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
        var voteCounts = new Dictionary<DateOnly, int>();
        while (reader.Read())
        {
            DateOnly date = (DateOnly)reader["voted_date"]; int voteCount = Convert.ToInt32(reader["vote_count"]);
            voteCounts[date] = voteCount;
        }
        return voteCounts;
    }

    private Meeting MapMeeting(IDataReader reader)
    {
        DateOnly? scheduledDate = reader.IsDBNull(reader.GetOrdinal("scheduled_date"))
            ? null
            : (DateOnly)reader["scheduled_date"];

        string? customThemeName = reader.IsDBNull(reader.GetOrdinal("custom_theme_name"))
            ? null
            : reader["custom_theme_name"].ToString();

        return new Meeting(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            ParseTheme(reader["theme"].ToString()!),
            customThemeName,
            TimeOnly.Parse(reader["meeting_time"].ToString()!),
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
        "custom" => MeetingTheme.Custom,
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

    public List<Meeting> GetByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT m.id, m.neighborhood_id, m.theme, m.custom_theme_name, m.meeting_time,
               m.date_range_start, m.date_range_end, m.status, m.scheduled_date
        FROM meetings m
        WHERE m.neighborhood_id = @neighborhoodId
        ORDER BY m.date_range_start";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        var meetings = new List<Meeting>();
        while (reader.Read())
            meetings.Add(MapMeeting(reader));
        return meetings;
    }

    public MeetingVote? GetVoteForCitizen(long meetingId, long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, meeting_id, citizen_id, voted_date
        FROM meeting_votes
        WHERE meeting_id = @meetingId AND citizen_id = @citizenId";

        AddParameter(command, "@meetingId", meetingId);
        AddParameter(command, "@citizenId", citizenId);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new MeetingVote(
                Convert.ToInt64(reader["id"]),
                Convert.ToInt64(reader["meeting_id"]),
                Convert.ToInt64(reader["citizen_id"]),
                (DateOnly)reader["voted_date"]
            );
        }
        return null;
    }

    public void UpdateVote(long voteId, DateOnly newDate)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE meeting_votes 
        SET voted_date = @votedDate 
        WHERE id = @id";

        AddParameter(command, "@id", voteId);
        AddParameter(command, "@votedDate", newDate.ToDateTime(TimeOnly.MinValue));
        command.ExecuteNonQuery();
    }
    public Meeting? GetById(long meetingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT m.id, m.neighborhood_id, m.theme, m.custom_theme_name, m.meeting_time,
               m.date_range_start, m.date_range_end, m.status, m.scheduled_date
        FROM meetings m
        WHERE m.id = @meetingId";

        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
            return MapMeeting(reader);
        return null;
    }
    public void Update(Meeting meeting)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE meetings 
        SET status = @status, scheduled_date = @scheduledDate
        WHERE id = @id";

        AddParameter(command, "@id", meeting.Id);
        AddParameter(command, "@status", ParseStatusToString(meeting.Status));
        IDbDataParameter scheduledDateParam = command.CreateParameter();
        scheduledDateParam.ParameterName = "@scheduledDate";
        scheduledDateParam.Value = meeting.ScheduledDate.HasValue
            ? (object)DateTime.SpecifyKind(meeting.ScheduledDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
            : DBNull.Value;
        scheduledDateParam.DbType = DbType.DateTime;
        command.Parameters.Add(scheduledDateParam);
        command.ExecuteNonQuery();
    }
}
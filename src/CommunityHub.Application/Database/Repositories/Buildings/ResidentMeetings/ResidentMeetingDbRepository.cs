using CommunityHub.Application.Database.Mappers.Buildings.ResidentMeetings;
using CommunityHub.Application.Database.Readers.Buildings.ResidentMeetings;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.ResidentMeetings;

public class ResidentMeetingDbRepository : BaseDbRepository, IResidentMeetingRepository
{
    public List<ResidentMeeting> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
        {SelectMeetingsSql()}
        WHERE rm.building_id = @buildingId
          AND (@status IS NULL OR rm.status = @status::resident_meeting_status)
        ORDER BY rm.meeting_date, rm.meeting_time";

        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@status", GetStatusParameterValue(status));

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadMeetings(reader);
    }
    public List<ResidentMeeting> GetActiveByBuilding(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
        {SelectMeetingsSql()}
        WHERE rm.building_id = @buildingId
          AND rm.status <> 'cancelled'
        ORDER BY rm.meeting_date, rm.meeting_time";

        AddParameter(command, "@tenantId", 0L);
        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadMeetings(reader);
    }

    public ResidentMeeting? GetById(long meetingId, long tenantId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
        {SelectMeetingsSql()}
        WHERE rm.id = @meetingId";

        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadSingleMeeting(reader);
    }

    public ResidentMeetingAttendance? GetAttendance(long meetingId, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT id, meeting_id, tenant_id, unit_number, created_at
        FROM resident_meeting_attendances
        WHERE meeting_id = @meetingId
          AND unit_number = @unitNumber";

        AddParameter(command, "@meetingId", meetingId);
        AddParameter(command, "@unitNumber", unitNumber);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadSingleAttendance(reader);
    }

    public void CreateAttendance(ResidentMeetingAttendance attendance)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO resident_meeting_attendances
            (meeting_id, tenant_id, unit_number, created_at)
        VALUES
            (@meetingId, @tenantId, @unitNumber, @createdAt)";

        AddParameter(command, "@meetingId", attendance.MeetingId);
        AddParameter(command, "@tenantId", attendance.TenantId);
        AddParameter(command, "@unitNumber", attendance.UnitNumber);
        AddTimestampParameter(command, "@createdAt", attendance.CreatedAt);

        command.ExecuteNonQuery();
    }

    public void DeleteAttendance(long meetingId, string unitNumber)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        DELETE FROM resident_meeting_attendances
        WHERE meeting_id = @meetingId
          AND unit_number = @unitNumber";

        AddParameter(command, "@meetingId", meetingId);
        AddParameter(command, "@unitNumber", unitNumber);

        command.ExecuteNonQuery();
    }

    public void CreateTopicSuggestion(ResidentMeetingTopicSuggestion suggestion)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO resident_meeting_topic_suggestions
            (meeting_id, tenant_id, topic, suggested_at)
        VALUES
            (@meetingId, @tenantId, @topic, @suggestedAt)";

        AddParameter(command, "@meetingId", suggestion.MeetingId);
        AddParameter(command, "@tenantId", suggestion.TenantId);
        AddParameter(command, "@topic", suggestion.Topic);
        AddTimestampParameter(command, "@suggestedAt", suggestion.SuggestedAt);

        command.ExecuteNonQuery();
    }

    public void UpdateStatus(long meetingId, ResidentMeetingStatus status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE resident_meetings
        SET status = @status::resident_meeting_status
        WHERE id = @meetingId";

        AddParameter(command, "@meetingId", meetingId);
        AddParameter(command, "@status", ResidentMeetingStatusMapper.ToDatabaseValue(status));

        command.ExecuteNonQuery();
    }

    public List<ResidentMeeting> GetAllByBuilding(long buildingId, ResidentMeetingStatus? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
        {SelectMeetingsSql()}
        WHERE rm.building_id = @buildingId
          AND (@status IS NULL OR rm.status = @status::resident_meeting_status)
        ORDER BY rm.meeting_date, rm.meeting_time";

        AddParameter(command, "@tenantId", 0L);
        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@status", GetStatusParameterValue(status));

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadMeetings(reader);
    }

    public int CountByBuilding(long buildingId, ResidentMeetingStatus? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT COUNT(*)
        FROM resident_meetings rm
        WHERE rm.building_id = @buildingId
          AND (@status IS NULL OR rm.status = @status::resident_meeting_status)";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@status", GetStatusParameterValue(status));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public long CreateMeeting(long buildingId, DateTime date, TimeSpan time)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO resident_meetings (building_id, meeting_date, meeting_time, status)
        VALUES (@buildingId, @date, @time, 'scheduled'::resident_meeting_status)
        RETURNING id";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@date", date.Date);
        AddParameter(command, "@time", time);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void AddTopic(long meetingId, string topic)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO resident_meeting_topics (meeting_id, topic)
        VALUES (@meetingId, @topic)";

        AddParameter(command, "@meetingId", meetingId);
        AddParameter(command, "@topic", topic);

        command.ExecuteNonQuery();
    }

    public List<ResidentMeetingTopicSuggestion> GetTopicSuggestions(long meetingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT id, meeting_id, tenant_id, topic, suggested_at
        FROM resident_meeting_topic_suggestions
        WHERE meeting_id = @meetingId
        ORDER BY suggested_at";

        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadTopicSuggestions(reader);
    }

    public List<ResidentMeetingAttendance> GetAttendances(long meetingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT id, meeting_id, tenant_id, unit_number, created_at
        FROM resident_meeting_attendances
        WHERE meeting_id = @meetingId
        ORDER BY unit_number";

        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadAttendances(reader);
    }

    public bool HasConflict(long buildingId, DateTime date, TimeSpan time)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT COUNT(*)
        FROM resident_meetings
        WHERE building_id = @buildingId
          AND meeting_date = @date
          AND meeting_time = @time
          AND status <> 'cancelled'::resident_meeting_status";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@date", date.Date);
        AddParameter(command, "@time", time);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    private static void AddTimestampParameter(IDbCommand command, string name, DateTime value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value.Kind == DateTimeKind.Utc
            ? value
            : value.ToUniversalTime();
        dbParam.DbType = DbType.DateTime;
        command.Parameters.Add(dbParam);
    }

    private static string SelectMeetingsSql()
    {
        return @"
        SELECT rm.id,
               rm.building_id,
               rm.meeting_date,
               rm.meeting_time,
               rm.status,
               COALESCE((
                   SELECT string_agg(rmt.topic, '|||' ORDER BY rmt.id)
                   FROM resident_meeting_topics rmt
                   WHERE rmt.meeting_id = rm.id
               ), '') AS topics,
               (
                   SELECT COUNT(*)
                   FROM units u
                   JOIN floors f ON u.floor_id = f.id
                   WHERE f.building_id = rm.building_id
               ) AS unit_count,
               (
                   SELECT COUNT(*)
                   FROM resident_meeting_attendances rma
                   WHERE rma.meeting_id = rm.id
               ) AS attendance_count,
               EXISTS(
                   SELECT 1
                   FROM resident_meeting_attendances rma
                   JOIN building_memberships bm
                     ON bm.building_id = rm.building_id
                    AND bm.user_id = @tenantId
                    AND bm.unit_number = rma.unit_number
                   WHERE rma.meeting_id = rm.id
               ) AS is_tenant_attending
        FROM resident_meetings rm";
    }

    private static string? GetStatusParameterValue(ResidentMeetingStatus? status)
    {
        return status.HasValue
            ? ResidentMeetingStatusMapper.ToDatabaseValue(status.Value)
            : null;
    }
}
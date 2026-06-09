using CommunityHub.Application.Database.Mappers.Buildings.ResidentMeetings;
using CommunityHub.Application.Database.Readers.Buildings.ResidentMeetings;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.ResidentMeetings;

public partial class ResidentMeetingDbRepository : BaseDbRepository, IResidentMeetingRepository
{
    public List<ResidentMeeting> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        string unitNumber,
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
        AddParameter(command, "@unitNumber", unitNumber);
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
        AddParameter(command, "@unitNumber", string.Empty);
        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadMeetings(reader);
    }

    public ResidentMeeting? GetById(long meetingId, long tenantId = 0L)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
        {SelectMeetingsSql()}
        WHERE rm.id = @meetingId";

        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@unitNumber", string.Empty);
        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadSingleMeeting(reader);
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
        AddParameter(command, "@unitNumber", string.Empty);
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
                    WHERE rma.meeting_id = rm.id
                    AND rma.unit_number = @unitNumber
               ) AS is_tenant_attending
        FROM resident_meetings rm";
    }

    private static string? GetStatusParameterValue(ResidentMeetingStatus? status)
    {
        return status.HasValue
            ? ResidentMeetingStatusMapper.ToDatabaseValue(status.Value)
            : null;
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
}
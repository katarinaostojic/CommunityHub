using CommunityHub.Application.Database.Readers.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.ResidentMeetings;

public partial class ResidentMeetingDbRepository
{
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
}
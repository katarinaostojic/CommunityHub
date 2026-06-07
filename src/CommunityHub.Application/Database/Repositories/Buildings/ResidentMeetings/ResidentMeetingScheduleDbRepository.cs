using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.ResidentMeetings;

public partial class ResidentMeetingDbRepository
{
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
}
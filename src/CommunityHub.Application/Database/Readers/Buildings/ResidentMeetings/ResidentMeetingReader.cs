using CommunityHub.Application.Database.Mappers.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings.ResidentMeetings;

public static class ResidentMeetingReader
{
    private const string TopicSeparator = "|||";

    public static List<ResidentMeeting> ReadMeetings(IDataReader reader)
    {
        List<ResidentMeeting> meetings = new();

        while (reader.Read())
        {
            meetings.Add(ReadMeeting(reader));
        }

        return meetings;
    }

    public static ResidentMeeting? ReadSingleMeeting(IDataReader reader)
    {
        if (!reader.Read())
            return null;

        return ReadMeeting(reader);
    }

    public static ResidentMeetingAttendance? ReadSingleAttendance(IDataReader reader)
    {
        if (!reader.Read())
            return null;

        return new ResidentMeetingAttendance(
            id: Convert.ToInt64(reader["id"]),
            meetingId: Convert.ToInt64(reader["meeting_id"]),
            tenantId: Convert.ToInt64(reader["tenant_id"]),
            unitNumber: reader["unit_number"].ToString()!,
            createdAt: ReadDateTime(reader, "created_at"));
    }

    private static ResidentMeeting ReadMeeting(IDataReader reader)
    {
        return new ResidentMeeting(
            id: Convert.ToInt64(reader["id"]),
            buildingId: Convert.ToInt64(reader["building_id"]),
            meetingDate: ReadDateTime(reader, "meeting_date"),
            meetingTime: ReadTimeSpan(reader, "meeting_time"),
            topics: ReadTopics(reader["topics"].ToString() ?? string.Empty),
            status: ResidentMeetingStatusMapper.FromDatabaseValue(reader["status"].ToString()!),
            attendanceCount: Convert.ToInt32(reader["attendance_count"]),
            unitCount: Convert.ToInt32(reader["unit_count"]),
            isTenantAttending: Convert.ToBoolean(reader["is_tenant_attending"]));
    }

    private static List<string> ReadTopics(string topics)
    {
        if (string.IsNullOrWhiteSpace(topics))
            return new List<string>();

        return topics
            .Split(TopicSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToList();
    }

    private static DateTime ReadDateTime(IDataReader reader, string columnName)
    {
        object value = reader[columnName];

        return value switch
        {
            DateTime dateTime => dateTime,
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            _ => DateTime.Parse(value.ToString()!)
        };
    }

    private static TimeSpan ReadTimeSpan(IDataReader reader, string columnName)
    {
        object value = reader[columnName];

        return value switch
        {
            TimeSpan timeSpan => timeSpan,
            TimeOnly timeOnly => timeOnly.ToTimeSpan(),
            _ => TimeSpan.Parse(value.ToString()!)
        };
    }
}
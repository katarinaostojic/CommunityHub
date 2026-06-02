namespace CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

public class ResidentMeeting
{
    private const int DeadlineHours = 24;

    public long Id { get; private set; }
    public long BuildingId { get; private set; }
    public DateTime MeetingDate { get; private set; }
    public TimeSpan MeetingTime { get; private set; }
    public List<string> Topics { get; private set; }
    public ResidentMeetingStatus Status { get; private set; }
    public int AttendanceCount { get; private set; }
    public int UnitCount { get; private set; }
    public bool IsTenantAttending { get; private set; }

    public ResidentMeeting(
        long id,
        long buildingId,
        DateTime meetingDate,
        TimeSpan meetingTime,
        IEnumerable<string> topics,
        ResidentMeetingStatus status,
        int attendanceCount,
        int unitCount,
        bool isTenantAttending)
    {
        Id = id;
        BuildingId = buildingId;
        MeetingDate = meetingDate.Date;
        MeetingTime = meetingTime;
        Topics = topics.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        Status = status;
        AttendanceCount = attendanceCount;
        UnitCount = unitCount;
        IsTenantAttending = isTenantAttending;
    }

    public DateTime ScheduledAt => MeetingDate.Date.Add(MeetingTime);

    public DateTime DeadlineAt => ScheduledAt.AddHours(-DeadlineHours);

    public bool HasQuorum => AttendanceCount * 2 > UnitCount;

    public bool CanChangeAttendance(DateTime now)
    {
        return Status != ResidentMeetingStatus.Cancelled && now < DeadlineAt;
    }

    public bool CanSuggestTopic(DateTime now)
    {
        return Status != ResidentMeetingStatus.Cancelled && now < DeadlineAt;
    }

    public void EnsureAttendanceCanBeChanged(DateTime now)
    {
        if (!CanChangeAttendance(now))
            throw new InvalidOperationException("Attendance can be changed up to 24h before the meeting.");
    }

    public void EnsureTopicCanBeSuggested(DateTime now)
    {
        if (!CanSuggestTopic(now))
            throw new InvalidOperationException("Topics can be suggested up to 24h before the meeting.");
    }

    public static string? ValidateTopic(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return "Topic is required.";

        if (topic.Trim().Length < 5)
            return "Topic must contain at least 5 characters.";

        if (topic.Trim().Length > 500)
            return "Topic cannot be longer than 500 characters.";

        return null;
    }
}
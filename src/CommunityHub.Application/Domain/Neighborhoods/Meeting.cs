namespace CommunityHub.Application.Domain.Neighborhoods;

public enum MeetingTheme
{
    Welcome,
    Motivation,
    Custom
}

public enum MeetingStatus
{
    InPreparation,
    Scheduled,
    Cancelled
}

public class Meeting
{
    public long Id { get; private set; }
    public long NeighborhoodId { get; private set; }
    public MeetingTheme Theme { get; private set; }
    public string? CustomThemeName { get; private set; }
    public TimeOnly MeetingTime { get; private set; }
    public DateOnly DateRangeStart { get; private set; }
    public DateOnly DateRangeEnd { get; private set; }
    public MeetingStatus Status { get; private set; }
    public DateOnly? ScheduledDate { get; private set; }

    public Meeting(long id, long neighborhoodId, MeetingTheme theme, string? customThemeName,
        TimeOnly meetingTime, DateOnly dateRangeStart, DateOnly dateRangeEnd,
        MeetingStatus status, DateOnly? scheduledDate)
    {
        Id = id;
        NeighborhoodId = neighborhoodId;
        Theme = theme;
        CustomThemeName = customThemeName;
        MeetingTime = meetingTime;
        DateRangeStart = dateRangeStart;
        DateRangeEnd = dateRangeEnd;
        Status = status;
        ScheduledDate = scheduledDate;
    }

    public Meeting(long neighborhoodId, MeetingTheme theme, string? customThemeName,
        TimeOnly meetingTime, DateOnly dateRangeStart, DateOnly dateRangeEnd)
    {
        Id = 0;
        NeighborhoodId = neighborhoodId;
        Theme = theme;
        CustomThemeName = customThemeName;
        MeetingTime = meetingTime;
        DateRangeStart = dateRangeStart;
        DateRangeEnd = dateRangeEnd;
        Status = MeetingStatus.InPreparation;
        ScheduledDate = null;
    }

    public void Schedule(DateOnly date)
    {
        Status = MeetingStatus.Scheduled;
        ScheduledDate = date;
    }

    public void Cancel()
    {
        Status = MeetingStatus.Cancelled;
    }
}
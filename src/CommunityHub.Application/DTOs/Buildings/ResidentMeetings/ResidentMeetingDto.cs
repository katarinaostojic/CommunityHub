using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

namespace CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

public class ResidentMeetingDto
{
    public long Id { get; init; }
    public long BuildingId { get; init; }
    public DateTime MeetingDate { get; init; }
    public TimeSpan MeetingTime { get; init; }
    public List<string> Topics { get; init; }
    public ResidentMeetingStatus Status { get; init; }
    public int AttendanceCount { get; init; }
    public int UnitCount { get; init; }
    public bool IsTenantAttending { get; init; }
    public DateTime DeadlineAt { get; init; }
    public bool CanAttend { get; init; }
    public bool CanCancelAttendance { get; init; }
    public bool CanSuggestTopic { get; init; }

    public ResidentMeetingDto(
        long id,
        long buildingId,
        DateTime meetingDate,
        TimeSpan meetingTime,
        List<string> topics,
        ResidentMeetingStatus status,
        int attendanceCount,
        int unitCount,
        bool isTenantAttending,
        DateTime deadlineAt,
        bool canAttend,
        bool canCancelAttendance,
        bool canSuggestTopic)
    {
        Id = id;
        BuildingId = buildingId;
        MeetingDate = meetingDate;
        MeetingTime = meetingTime;
        Topics = topics;
        Status = status;
        AttendanceCount = attendanceCount;
        UnitCount = unitCount;
        IsTenantAttending = isTenantAttending;
        DeadlineAt = deadlineAt;
        CanAttend = canAttend;
        CanCancelAttendance = canCancelAttendance;
        CanSuggestTopic = canSuggestTopic;
    }

    public double AttendancePercentage => UnitCount == 0
        ? 0
        : AttendanceCount * 100.0 / UnitCount;
}
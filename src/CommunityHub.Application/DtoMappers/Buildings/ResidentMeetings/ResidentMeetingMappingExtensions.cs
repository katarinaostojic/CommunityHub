using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Mappings.Buildings.ResidentMeetings;

public static class ResidentMeetingMappingExtensions
{
    public static ResidentMeetingDto ToDto(this ResidentMeeting meeting, DateTime now)
    {
        bool canChangeAttendance = meeting.CanChangeAttendance(now);
        bool canSuggestTopic = meeting.CanSuggestTopic(now);

        return new ResidentMeetingDto(
            id: meeting.Id,
            buildingId: meeting.BuildingId,
            meetingDate: meeting.MeetingDate,
            meetingTime: meeting.MeetingTime,
            topics: meeting.Topics,
            status: meeting.Status,
            attendanceCount: meeting.AttendanceCount,
            unitCount: meeting.UnitCount,
            isTenantAttending: meeting.IsTenantAttending,
            deadlineAt: meeting.DeadlineAt,
            canAttend: canChangeAttendance && !meeting.IsTenantAttending,
            canCancelAttendance: canChangeAttendance && meeting.IsTenantAttending,
            canSuggestTopic: canSuggestTopic);
    }

    public static List<ResidentMeetingDto> ToDtoList(
        this IEnumerable<ResidentMeeting> meetings,
        DateTime now)
    {
        return meetings.Select(m => m.ToDto(now)).ToList();
    }
}
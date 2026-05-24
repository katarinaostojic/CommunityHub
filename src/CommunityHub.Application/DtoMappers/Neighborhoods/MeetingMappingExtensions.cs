using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class MeetingMappingExtensions
{
    public static MeetingDto ToDto(this Meeting meeting, bool canVote,
        DateOnly? citizenVotedDate = null, long? citizenVoteId = null)
    {
        return new MeetingDto(
            id: meeting.Id,
            theme: meeting.Theme switch
            {
                MeetingTheme.Welcome => "welcome",
                MeetingTheme.Motivation => "motivation",
                MeetingTheme.Custom => "custom",
                _ => "welcome"
            },
            customThemeName: meeting.Theme == MeetingTheme.Custom ? meeting.CustomThemeName : null,
            meetingTime: meeting.MeetingTime.ToString("HH:mm"),
            dateRangeStart: meeting.DateRangeStart.ToString("dd/MM/yyyy"),
            dateRangeEnd: meeting.DateRangeEnd.ToString("dd/MM/yyyy"),
            status: meeting.Status switch
            {
                MeetingStatus.InPreparation => "in_preparation",
                MeetingStatus.Scheduled => "scheduled",
                MeetingStatus.Cancelled => "cancelled",
                _ => "in_preparation"
            },
            scheduledDate: meeting.ScheduledDate?.ToString("dd/MM/yyyy"),
            canVote: canVote,
            citizenVotedDate: citizenVotedDate,
            citizenVoteId: citizenVoteId
        );
    }

    public static List<MeetingDto> ToDtoList(this IEnumerable<Meeting> meetings,
        Func<Meeting, bool> canVoteFunc,
        Func<Meeting, (DateOnly? votedDate, long? voteId)> getVoteFunc)
    {
        return meetings.Select(m =>
        {
            var (votedDate, voteId) = getVoteFunc(m);
            return m.ToDto(canVoteFunc(m), votedDate, voteId);
        }).ToList();
    }
}
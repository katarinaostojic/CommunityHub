using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class MeetingViewModel
{
    private readonly Meeting _meeting;

    public MeetingViewModel(Meeting meeting)
    {
        _meeting = meeting;
    }

    public long MeetingId => _meeting.Id;
    public bool IsInPreparation => _meeting.Status == MeetingStatus.InPreparation;
    public bool HasTiedVotes { get; set; }
    public Dictionary<DateOnly, int> VoteCounts { get; set; } = new();

    public string TopicDisplay => _meeting.Theme switch
    {
        MeetingTheme.Welcome => "Topic: Welcome Meeting",
        MeetingTheme.Motivation => "Topic: Community Motivation",
        MeetingTheme.Custom => $"Topic: {_meeting.CustomThemeName}",
        _ => "Topic: Unknown"
    };

    public string DateDisplay => _meeting.Status == MeetingStatus.Scheduled && _meeting.ScheduledDate.HasValue
        ? $"Date: {_meeting.ScheduledDate.Value:dd.MM.yyyy}."
        : $"Date Range: {_meeting.DateRangeStart:dd.MM.yyyy} - {_meeting.DateRangeEnd:dd.MM.yyyy}";

    public string TimeDisplay => $"Time: {_meeting.MeetingTime:HH.mm}h";

    public string StatusDisplay => _meeting.Status switch
    {
        MeetingStatus.InPreparation => "Status: In Preparation",
        MeetingStatus.Scheduled => "Status: Scheduled",
        MeetingStatus.Cancelled => "Status: Cancelled",
        _ => _meeting.Status.ToString()
    };
}
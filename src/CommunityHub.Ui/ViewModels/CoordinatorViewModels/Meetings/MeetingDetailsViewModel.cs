using CommunityHub.Application.Domain.Entities.Neighborhoods.Meetings;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class MeetingDetailsViewModel : BaseViewModel
{
    private readonly Meeting _meeting;
    private readonly Dictionary<DateOnly, int> _voteCounts;

    public MeetingDetailsViewModel(MeetingService meetingService, long meetingId)
    {
        _meeting = meetingService.GetById(meetingId)!;
        _voteCounts = meetingService.GetVoteCounts(meetingId);
        LoadVoteItems();
    }

    public string TopicDisplay => _meeting.Theme switch
    {
        MeetingTheme.Welcome => "Topic: Welcome Meeting",
        MeetingTheme.Motivation => "Topic: Community Motivation",
        MeetingTheme.Custom => $"Topic: {_meeting.CustomThemeName}",
        _ => "Topic: Unknown"
    };

    public string DateDisplay => _meeting.Status == MeetingStatus.Scheduled && _meeting.ScheduledDate.HasValue
        ? $"Date: {_meeting.ScheduledDate.Value:dd.MM.yyyy}."
        : $"Date: {_meeting.DateRangeStart:dd.MM.yyyy}.";

    public string DateRangeDisplay =>
        $"Date range: {_meeting.DateRangeStart:dd.MM.yyyy} - {_meeting.DateRangeEnd:dd.MM.yyyy}";

    public string TimeDisplay => $"Time: {_meeting.MeetingTime:HH.mm}h";

    public string StatusDisplay => _meeting.Status switch
    {
        MeetingStatus.InPreparation => "Status: In Preparation",
        MeetingStatus.Scheduled => "Status: Scheduled",
        MeetingStatus.Cancelled => "Status: Cancelled",
        _ => _meeting.Status.ToString()
    };

    public List<VoteItem> VoteItems { get; private set; } = new();

    private void LoadVoteItems()
    {
        if (_voteCounts.Count == 0)
        {
            VoteItems = new List<VoteItem>();
            return;
        }

        int maxVotes = _voteCounts.Values.Max();

        VoteItems = _voteCounts
            .OrderByDescending(v => v.Value)
            .Select(v => new VoteItem
            {
                DateLabel = v.Key.ToString("dd.MM.yyyy."),
                VotesLabel = $"{v.Value} votes",
                Percentage = maxVotes > 0 ? (double)v.Value / maxVotes * 100 : 0
            })
            .ToList();
    }
}

public class VoteItem
{
    public string DateLabel { get; set; } = string.Empty;
    public string VotesLabel { get; set; } = string.Empty;
    public double Percentage { get; set; }
}
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;
using CommunityHub.Application.Services;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class MeetingsViewModel : BaseViewModel
{
    private readonly MeetingService _meetingService;
    private readonly StatisticsService _statisticsService;
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;

    private ObservableCollection<MeetingViewModel> _meetings = new();
    private MeetingStatus? _currentFilter = null;
    private int _newCount;
    private int _inactiveCount;
    private int _activeCount;
    private int _distinguishedCount;
    private int _trustedCount;
    private string _suggestionText = string.Empty;

    public MeetingsViewModel(MeetingService meetingService, StatisticsService statisticsService,
        long coordinatorId, long neighborhoodId)
    {
        _meetingService = meetingService;
        _statisticsService = statisticsService;
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;
        LoadStatistics();
        LoadMeetings();
    }

    public ObservableCollection<MeetingViewModel> Meetings
    {
        get => _meetings;
        private set => SetProperty(ref _meetings, value);
    }

    public int NewCount
    {
        get => _newCount;
        private set => SetProperty(ref _newCount, value);
    }

    public int InactiveCount
    {
        get => _inactiveCount;
        private set => SetProperty(ref _inactiveCount, value);
    }

    public int ActiveCount
    {
        get => _activeCount;
        private set => SetProperty(ref _activeCount, value);
    }

    public int DistinguishedCount
    {
        get => _distinguishedCount;
        private set => SetProperty(ref _distinguishedCount, value);
    }

    public int TrustedCount
    {
        get => _trustedCount;
        private set => SetProperty(ref _trustedCount, value);
    }

    public string SuggestionText
    {
        get => _suggestionText;
        private set => SetProperty(ref _suggestionText, value);
    }

    public void FilterAll()
    {
        _currentFilter = null;
        LoadMeetings();
    }

    public void FilterScheduled()
    {
        _currentFilter = MeetingStatus.Scheduled;
        LoadMeetings();
    }

    public void FilterCancelled()
    {
        _currentFilter = MeetingStatus.Cancelled;
        LoadMeetings();
    }

    public void FilterInPreparation()
    {
        _currentFilter = MeetingStatus.InPreparation;
        LoadMeetings();
    }

    public void FinalizeVoting(long meetingId)
    {
        _meetingService.CheckAndFinalizeVoting(meetingId);
        LoadMeetings();
    }

    public bool MeetingHasTiedVotes(long meetingId)
        => _meetingService.HasTiedVotes(meetingId);

    public Dictionary<DateOnly, int> GetVoteCounts(long meetingId)
        => _meetingService.GetVoteCounts(meetingId);

    public void FinalizeWithDate(long meetingId, DateOnly chosenDate)
    {
        _meetingService.ScheduleWithDate(meetingId, chosenDate);
        LoadMeetings();
    }

    private void LoadStatistics()
    {
        if (_neighborhoodId == -1)
        {
            SuggestionText = string.Empty;
            return;
        }

        var trustStatistics = _statisticsService.GetTrustStatistics(_neighborhoodId);
        NewCount = trustStatistics.GetValueOrDefault(TrustLevel.New, 0);
        InactiveCount = trustStatistics.GetValueOrDefault(TrustLevel.Inactive, 0);
        ActiveCount = trustStatistics.GetValueOrDefault(TrustLevel.Active, 0);
        DistinguishedCount = trustStatistics.GetValueOrDefault(TrustLevel.Distinguished, 0);
        TrustedCount = trustStatistics.GetValueOrDefault(TrustLevel.Trusted, 0);

        var meetingThemeSuggestion = _statisticsService.SuggestMeetingTheme(_neighborhoodId);
        SuggestionText = meetingThemeSuggestion == MeetingTheme.Welcome
            ? "Suggestion: Organize a welcome meeting"
            : meetingThemeSuggestion == MeetingTheme.Motivation
                ? "Suggestion: Organize a motivation meeting"
                : "No suggestion at this time.";
    }

    private void LoadMeetings()
    {
        var meetings = _meetingService.GetMeetingsByCoordinator(_coordinatorId);

        foreach (var meeting in meetings.Where(m => m.Status == MeetingStatus.InPreparation))
        {
            DateTime deadline = meeting.DateRangeStart.ToDateTime(TimeOnly.MinValue).AddHours(-24);
            if (DateTime.Now >= deadline)
            {
                if (!_meetingService.HasTiedVotes(meeting.Id))
                    _meetingService.CheckAndFinalizeVoting(meeting.Id);
            }
        }

        meetings = _meetingService.GetMeetingsByCoordinator(_coordinatorId);

        if (_currentFilter.HasValue)
            meetings = meetings.Where(m => m.Status == _currentFilter.Value).ToList();

        Meetings = new ObservableCollection<MeetingViewModel>(
            meetings.Select(m => new MeetingViewModel(m)
            {
                HasTiedVotes = m.Status == MeetingStatus.InPreparation && _meetingService.HasTiedVotes(m.Id),
                VoteCounts = m.Status == MeetingStatus.InPreparation ? _meetingService.GetVoteCounts(m.Id) : new()
            }).ToList());
    }
}
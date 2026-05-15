using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;

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

    private void LoadStatistics()
    {
        if (_neighborhoodId == -1)
        {
            SuggestionText = string.Empty;
            return;
        }

        var stats = _statisticsService.GetTrustStatistics(_neighborhoodId);
        NewCount = stats.GetValueOrDefault(TrustLevel.New, 0);
        InactiveCount = stats.GetValueOrDefault(TrustLevel.Inactive, 0);
        ActiveCount = stats.GetValueOrDefault(TrustLevel.Active, 0);
        DistinguishedCount = stats.GetValueOrDefault(TrustLevel.Distinguished, 0);
        TrustedCount = stats.GetValueOrDefault(TrustLevel.Trusted, 0);

        var suggestion = _statisticsService.SuggestMeetingTheme(_neighborhoodId);
        SuggestionText = suggestion == MeetingTheme.Welcome
            ? "Suggestion: Organize a welcome meeting"
            : suggestion == MeetingTheme.Motivation
                ? "Suggestion: Organize a motivation meeting"
                : "No suggestion at this time.";
    }

    private void LoadMeetings()
    {
        var meetings = _meetingService.GetMeetingsByCoordinator(_coordinatorId);

        if (_currentFilter.HasValue)
            meetings = meetings.Where(m => m.Status == _currentFilter.Value).ToList();

        Meetings = new ObservableCollection<MeetingViewModel>(
            meetings.Select(m => new MeetingViewModel(m)).ToList());
    }
}
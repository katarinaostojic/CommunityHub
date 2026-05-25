using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

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
    private SeriesCollection _trustSeriesCollection = new();
    private List<TrustStatItem> _trustStats = new();

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

    public Visibility SuggestionLinkVisibility =>
        SuggestionText != "No suggestion at this time." && !string.IsNullOrEmpty(SuggestionText)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public SeriesCollection TrustSeriesCollection
    {
        get => _trustSeriesCollection;
        private set => SetProperty(ref _trustSeriesCollection, value);
    }

    public List<TrustStatItem> TrustStats
    {
        get => _trustStats;
        private set => SetProperty(ref _trustStats, value);
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
        if (_neighborhoodId == 0)
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

        OnPropertyChanged(nameof(SuggestionLinkVisibility));

        TrustSeriesCollection = new SeriesCollection
        {
            new PieSeries { Title = "Nov u kvartu",            Values = new ChartValues<int> { NewCount },           Fill = new SolidColorBrush(Color.FromRgb(94, 163, 110)),   StrokeThickness = 0 },
            new PieSeries { Title = "Neaktivan gradjanin",     Values = new ChartValues<int> { InactiveCount },      Fill = new SolidColorBrush(Color.FromRgb(158, 225, 203)),  StrokeThickness = 0 },
            new PieSeries { Title = "Aktivan gradjanin",       Values = new ChartValues<int> { ActiveCount },        Fill = new SolidColorBrush(Color.FromRgb(29, 158, 117)),   StrokeThickness = 0 },
            new PieSeries { Title = "Istaknut gradjanin",      Values = new ChartValues<int> { DistinguishedCount }, Fill = new SolidColorBrush(Color.FromRgb(15, 110, 86)),    StrokeThickness = 0 },
            new PieSeries { Title = "Gradjanin od povjerenja", Values = new ChartValues<int> { TrustedCount },       Fill = new SolidColorBrush(Color.FromRgb(8, 80, 65)),      StrokeThickness = 0 },
        };

        TrustStats = new List<TrustStatItem>
        {
            new("Nov u kvartu",            NewCount,          new SolidColorBrush(Color.FromRgb(94, 163, 110))),
            new("Neaktivan gradjanin",     InactiveCount,     new SolidColorBrush(Color.FromRgb(158, 225, 203))),
            new("Aktivan gradjanin",       ActiveCount,       new SolidColorBrush(Color.FromRgb(29, 158, 117))),
            new("Istaknut gradjanin",      DistinguishedCount,new SolidColorBrush(Color.FromRgb(15, 110, 86))),
            new("Gradjanin od povjerenja", TrustedCount,      new SolidColorBrush(Color.FromRgb(8, 80, 65))),
        };
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

public class TrustStatItem
{
    public string Label { get; }
    public int Count { get; }
    public SolidColorBrush Color { get; }

    public TrustStatItem(string label, int count, SolidColorBrush color)
    {
        Label = label;
        Count = count;
        Color = color;
    }
}
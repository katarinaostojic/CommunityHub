using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class MeetingsPage : Page
{
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;
    private readonly MeetingService _meetingService;
    private readonly StatisticsService _statisticsService;
    private MeetingStatus? _currentFilter = null;

    public MeetingsPage(long coordinatorId, long neighborhoodId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;
        _meetingService = new MeetingService();
        _statisticsService = new StatisticsService();
        LoadStatistics();
        LoadMeetings();
    }

    private void LoadStatistics()
    {
        var stats = _statisticsService.GetTrustStatistics(_neighborhoodId);
        NewCount.Text = stats.GetValueOrDefault(TrustLevel.New, 0).ToString();
        InactiveCount.Text = stats.GetValueOrDefault(TrustLevel.Inactive, 0).ToString();
        ActiveCount.Text = stats.GetValueOrDefault(TrustLevel.Active, 0).ToString();
        DistinguishedCount.Text = stats.GetValueOrDefault(TrustLevel.Distinguished, 0).ToString();
        TrustedCount.Text = stats.GetValueOrDefault(TrustLevel.Trusted, 0).ToString();

        var suggestion = _statisticsService.SuggestMeetingTheme(_neighborhoodId);
        if (suggestion == MeetingTheme.Welcome)
            SuggestionText.Text = "Suggestion: Organize a welcome meeting";
        else if (suggestion == MeetingTheme.Motivation)
            SuggestionText.Text = "Suggestion: Organize a motivation meeting";
        else
            SuggestionText.Text = "No suggestion at this time.";
    }

    private void LoadMeetings()
    {
        var meetings = _meetingService.GetMeetingsByCoordinator(_coordinatorId);

        if (_currentFilter.HasValue)
            meetings = meetings.Where(m => m.Status == _currentFilter.Value).ToList();

        MeetingsItemsControl.ItemsSource = meetings
            .Select(m => new MeetingViewModel(m))
            .ToList();
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = null;
        LoadMeetings();
    }

    private void FilterScheduledButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = MeetingStatus.Scheduled;
        LoadMeetings();
    }

    private void FilterCancelledButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = MeetingStatus.Cancelled;
        LoadMeetings();
    }

    private void FilterInPreparationButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = MeetingStatus.InPreparation;
        LoadMeetings();
    }

    private void AddMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new AddNewMeetingPage(_coordinatorId, _neighborhoodId), "Add New Meeting");
    }
}

public class MeetingViewModel
{
    private readonly Meeting _meeting;

    public MeetingViewModel(Meeting meeting)
    {
        _meeting = meeting;
    }

    public string TopicDisplay => _meeting.Theme == MeetingTheme.Welcome
        ? "Topic: Welcome Meeting"
        : "Topic: Community Motivation";

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
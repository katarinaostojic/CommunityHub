using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class MeetingsPage : Page
{
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;
    private readonly MeetingsViewModel _viewModel;

    public MeetingsPage(long coordinatorId, long neighborhoodId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;
        MeetingService meetingService = Injector.CreateInstance<MeetingService>();
        StatisticsService statisticsService = Injector.CreateInstance<StatisticsService>();
        _viewModel = new MeetingsViewModel(meetingService, statisticsService, coordinatorId, neighborhoodId);
        DataContext = _viewModel;
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();
    private void FilterScheduledButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterScheduled();
    private void FilterCancelledButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterCancelled();
    private void FilterInPreparationButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterInPreparation();

    private void AddMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new AddNewMeetingPage(_coordinatorId, _neighborhoodId), "Add New Meeting");
    }

    private async void FinalizeVotingButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingViewModel = (MeetingViewModel)((Button)sender).Tag;

        if (meetingViewModel.HasTiedVotes)
        {
            CoordinatorMainWindow.Instance.NavigateTo(
                new SelectDatePage(_viewModel, meetingViewModel.MeetingId,
                    _coordinatorId, _neighborhoodId, meetingViewModel.VoteCounts),
                "Select Date");
            return;
        }

        try
        {
            _viewModel.FinalizeVoting(meetingViewModel.MeetingId);
            SuccessText.Text = "Voting finalized!";
            SuccessBanner.Visibility = Visibility.Visible;
            ErrorBanner.Visibility = Visibility.Collapsed;
            await Task.Delay(3000);
            SuccessBanner.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorBanner.Visibility = Visibility.Visible;
            SuccessBanner.Visibility = Visibility.Collapsed;
            await Task.Delay(3000);
            ErrorBanner.Visibility = Visibility.Collapsed;
        }
    }
    private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new GenerateReportPage(_coordinatorId), "Generate Report");
    }
    private void SuggestionLink_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new AddNewMeetingPage(_coordinatorId, _neighborhoodId), "Add New Meeting");
    }
    private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingViewModel = (MeetingViewModel)((Button)sender).Tag;
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingDetailsPage(meetingViewModel.MeetingId), "Meeting Details");
    }
}
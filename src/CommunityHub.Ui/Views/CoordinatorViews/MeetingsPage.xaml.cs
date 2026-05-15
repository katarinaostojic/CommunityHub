using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Neighborhoods;
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
}
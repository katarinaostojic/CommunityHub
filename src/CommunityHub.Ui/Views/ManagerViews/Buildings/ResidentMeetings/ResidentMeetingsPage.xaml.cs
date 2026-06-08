using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ResidentMeetings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs.Buildings.ResidentMeetings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews.Buildings.ResidentMeetings;

public partial class ResidentMeetingsPage : Page
{
    private readonly User _currentUser;
    private readonly ResidentMeetingsViewModel _viewModel;

    public event Action<BuildingDto>? BuildingSelected;

    public ResidentMeetingsPage(User user, BuildingDto? preselectedBuilding = null)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new ResidentMeetingsViewModel(_currentUser.Id);
        DataContext = _viewModel;

        if (preselectedBuilding != null)
        {
            Loaded += (s, e) =>
            {
                BuildingComboBox.SelectedItem = _viewModel.Buildings
                    .FirstOrDefault(b => b.Id == preselectedBuilding.Id);
            };
        }
    }

    private void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BuildingComboBox.SelectedItem is BuildingDto building)
        {
            _viewModel.SelectedBuilding = building;
            NoBuildingText.Visibility = Visibility.Collapsed;
            MeetingsList.Visibility = Visibility.Visible;
            BuildingSelected?.Invoke(building);
            RefreshEmptyState();
        }
        else
        {
            NoBuildingText.Visibility = Visibility.Visible;
            MeetingsList.Visibility = Visibility.Collapsed;
            NoMeetingsText.Visibility = Visibility.Collapsed;
        }
    }

    private void RefreshEmptyState()
    {
        if (_viewModel.Meetings.Count == 0)
        {
            NoMeetingsText.Visibility = Visibility.Visible;
            MeetingsList.Visibility = Visibility.Collapsed;
        }
        else
        {
            NoMeetingsText.Visibility = Visibility.Collapsed;
            MeetingsList.Visibility = Visibility.Visible;
        }
    }

    private void ScheduleMeeting_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedBuilding == null)
        {
            MessageBox.Show("Please select a building first.", "Error");
            return;
        }

        var dialog = new ScheduleMeetingDialog();
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() != true) return;

        try
        {
            _viewModel.ScheduleMeeting(
                dialog.SelectedDate!.Value,
                dialog.SelectedTime!.Value,
                dialog.Topics);
            RefreshEmptyState();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    private void ViewAttendance_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long meetingId)
        {
            var attendances = _viewModel.GetAttendances(meetingId);
            var dialog = new AttendanceDialog(attendances);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }

    private void ViewSuggestions_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ResidentMeetingRowViewModel vm)
        {
            var suggestions = _viewModel.GetTopicSuggestions(vm.Id);

            // Filtriraj one koje su vec dodate
            var filteredSuggestions = suggestions
                .Where(s => !vm.Topics.Contains(s.Topic))
                .ToList();

            // Ako su sve vec dodate ili nema sugestija — slobodno otvori
            bool alreadyAddedOne = suggestions.Any(s => vm.Topics.Contains(s.Topic));

            var dialog = new TopicSuggestionsDialog(
                filteredSuggestions,
                alreadyAddedOne,
                topic => _viewModel.AddTopicFromSuggestion(vm.Id, topic));
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }

    private void FilterAll_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.FilterAll();
        RefreshEmptyState();
    }
    private void FilterScheduled_Click(object sender, RoutedEventArgs e) 
    {   
        _viewModel.FilterScheduled();
        RefreshEmptyState();
    }
    private void FilterConfirmed_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.FilterConfirmed();
        RefreshEmptyState();
    }
    private void FilterCancelled_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.FilterCancelled();
        RefreshEmptyState();
    }

    private void ShowConfirmation(string message)
    {
        var dialog = new Dialogs.ConfirmationDialog(message);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
    }
}
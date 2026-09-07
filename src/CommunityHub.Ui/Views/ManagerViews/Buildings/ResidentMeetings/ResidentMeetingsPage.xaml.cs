using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ResidentMeetings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Dialogs.Buildings.ResidentMeetings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs.Buildings.ResidentMeetings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        SetActiveFilterButton(BtnAll);

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
            BuildingSelected?.Invoke(building);
        }
        else
        {
            _viewModel.SelectedBuilding = null;
        }

        RefreshEmptyState();
    }

    private void RefreshEmptyState()
    {
        if (_viewModel.SelectedBuilding == null)
        {
            NoBuildingText.Visibility = Visibility.Visible;
            NoMeetingsText.Visibility = Visibility.Collapsed;
            MeetingsList.Visibility = Visibility.Collapsed;
            return;
        }

        NoBuildingText.Visibility = Visibility.Collapsed;

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
            var dialogVm = new TopicSuggestionsDialogViewModel(_viewModel, vm);
            var dialog = new TopicSuggestionsDialog(dialogVm);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }

    private void FilterAll_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnAll);
        _viewModel.FilterAll();
        RefreshEmptyState();
    }
    private void FilterScheduled_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnScheduled);
        _viewModel.FilterScheduled();
        RefreshEmptyState();
    }
    private void FilterConfirmed_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnConfirmed);
        _viewModel.FilterConfirmed();
        RefreshEmptyState();
    }
    private void FilterCancelled_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnCancelled);
        _viewModel.FilterCancelled();
        RefreshEmptyState();
    }

    private void SetActiveFilterButton(Button activeButton)
    {
        var filterButtons = new[] { BtnAll, BtnScheduled, BtnConfirmed, BtnCancelled };
        foreach (var btn in filterButtons)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(0xE8, 0xED, 0xF2));
            btn.Foreground = new SolidColorBrush(Color.FromRgb(0x2C, 0x3E, 0x50));
        }

        activeButton.Background = new SolidColorBrush(Color.FromRgb(0x29, 0x80, 0xB9));
        activeButton.Foreground = Brushes.White;
    }

    private void ShowConfirmation(string message)
    {
        var dialog = new Dialogs.ConfirmationDialog(message);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
    }
}
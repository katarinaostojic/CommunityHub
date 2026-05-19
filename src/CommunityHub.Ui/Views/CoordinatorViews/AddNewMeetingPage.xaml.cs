using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class AddNewMeetingPage : Page
{
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;
    private readonly AddNewMeetingViewModel _viewModel;

    public AddNewMeetingPage(long coordinatorId, long neighborhoodId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;
        _viewModel = new AddNewMeetingViewModel(
            Injector.CreateInstance<MeetingService>(), neighborhoodId);
    }

    private void ScheduleMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        string themeInput = ThemeComboBox.SelectedItem != null
            ? ((ComboBoxItem)ThemeComboBox.SelectedItem).Content.ToString()!
            : ThemeComboBox.Text;

        string? validationError = _viewModel.Validate(
            themeInput,
            StartDatePicker.SelectedDate,
            EndDatePicker.SelectedDate,
            TimeTextBox.Text);

        if (validationError != null)
        {
            MessageBox.Show(validationError, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DateOnly startDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate!.Value);
        DateOnly endDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate!.Value);
        TimeOnly.TryParse(TimeTextBox.Text, out TimeOnly meetingTime);

        try
        {
            _viewModel.CreateMeeting(themeInput, meetingTime, startDate, endDate);
            MessageBox.Show("Meeting scheduled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            CoordinatorMainWindow.Instance.NavigateTo(
                new MeetingsPage(_coordinatorId, _neighborhoodId), "Meetings");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingsPage(_coordinatorId, _neighborhoodId), "Meetings");
    }
}
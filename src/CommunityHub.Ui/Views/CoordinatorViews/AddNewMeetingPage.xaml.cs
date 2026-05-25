using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
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

    private void StartCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) { }

    private void EndCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) { }

    private void ScheduleMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        string themeInput = ThemeComboBox.SelectedItem != null
            ? ((ComboBoxItem)ThemeComboBox.SelectedItem).Content.ToString()!
            : ThemeComboBox.Text;

        DateTime? startDate = StartCalendar.SelectedDate;
        DateTime? endDate = EndCalendar.SelectedDate;

        string? validationError = _viewModel.Validate(
            themeInput,
            startDate,
            endDate,
            TimeTextBox.Text);

        if (validationError != null)
        {
            MessageBox.Show(validationError, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DateOnly start = DateOnly.FromDateTime(startDate!.Value);
        DateOnly end = DateOnly.FromDateTime(endDate!.Value);
        TimeOnly.TryParse(TimeTextBox.Text, out TimeOnly meetingTime);

        try
        {
            _viewModel.CreateMeeting(themeInput, meetingTime, start, end);
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
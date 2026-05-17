using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class AddNewMeetingPage : Page
{
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;
    private readonly MeetingService _meetingService;

    public AddNewMeetingPage(long coordinatorId, long neighborhoodId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;
        _meetingService = Injector.CreateInstance<MeetingService>();
    }

    private void ScheduleMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a topic.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
        {
            MessageBox.Show("Please select start and end date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
        {
            MessageBox.Show("End date cannot be before start date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!TimeOnly.TryParse(TimeTextBox.Text, out TimeOnly meetingTime))
        {
            MessageBox.Show("Please enter a valid time (HH:mm).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MeetingTheme theme = ((ComboBoxItem)ThemeComboBox.SelectedItem).Content.ToString() == "Welcome"
            ? MeetingTheme.Welcome
            : MeetingTheme.Motivation;

        DateOnly startDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value);
        DateOnly endDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value);

        Meeting meeting = new Meeting(_neighborhoodId, theme, meetingTime, startDate, endDate);

        try
        {
            _meetingService.CreateMeeting(meeting);
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
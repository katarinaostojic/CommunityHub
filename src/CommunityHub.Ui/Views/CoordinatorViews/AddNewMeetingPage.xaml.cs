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

        TopicErrorText.Visibility = Visibility.Collapsed;
        DateErrorText.Visibility = Visibility.Collapsed;
        TimeErrorText.Visibility = Visibility.Collapsed;

        bool isValid = true;

        if (string.IsNullOrWhiteSpace(themeInput))
        {
            TopicErrorText.Text = "Please select or enter a topic.";
            TopicErrorText.Visibility = Visibility.Visible;
            isValid = false;
        }

        if (startDate == null || endDate == null)
        {
            DateErrorText.Text = "Please select start and end date.";
            DateErrorText.Visibility = Visibility.Visible;
            isValid = false;
        }
        else if (endDate < startDate)
        {
            DateErrorText.Text = "End date cannot be before start date.";
            DateErrorText.Visibility = Visibility.Visible;
            isValid = false;
        }

        if (!TimeOnly.TryParse(TimeTextBox.Text, out _))
        {
            TimeErrorText.Text = "Please enter a valid time (HH:mm).";
            TimeErrorText.Visibility = Visibility.Visible;
            isValid = false;
        }

        if (!isValid) return;

        DateOnly start = DateOnly.FromDateTime(startDate!.Value);
        DateOnly end = DateOnly.FromDateTime(endDate!.Value);
        TimeOnly.TryParse(TimeTextBox.Text, out TimeOnly meetingTime);

        try
        {
            _viewModel.CreateMeeting(themeInput, meetingTime, start, end);
            CoordinatorMainWindow.Instance.NavigateTo(
                new MeetingsPage(_coordinatorId, _neighborhoodId), "Meetings");
        }
        catch (Exception ex)
        {
            TimeErrorText.Text = ex.Message;
            TimeErrorText.Visibility = Visibility.Visible;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingsPage(_coordinatorId, _neighborhoodId), "Meetings");
    }
}
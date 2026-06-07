using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Meetings;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;
using CommunityHub.Application.Services.Reports;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class GenerateReportPage : Page
{
    private readonly long _coordinatorId;
    private readonly MeetingService _meetingService;

    public GenerateReportPage(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _meetingService = Injector.CreateInstance<MeetingService>();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (CoordinatorMainWindow.Instance.MainFrame.CanGoBack)
            CoordinatorMainWindow.Instance.MainFrame.GoBack();
    }

    private void GeneratePdfButton_Click(object sender, RoutedEventArgs e)
    {
        DateErrorText.Visibility = Visibility.Collapsed;

        if (StartCalendar.SelectedDate == null || EndCalendar.SelectedDate == null)
        {
            DateErrorText.Text = "Please select both start and end date.";
            DateErrorText.Visibility = Visibility.Visible;
            return;
        }

        DateOnly startDate = DateOnly.FromDateTime(StartCalendar.SelectedDate.Value);
        DateOnly endDate = DateOnly.FromDateTime(EndCalendar.SelectedDate.Value);

        if (startDate > endDate)
        {
            DateErrorText.Text = "Start date must be before end date.";
            DateErrorText.Visibility = Visibility.Visible;
            return;
        }

        string reportType = ((ComboBoxItem)ReportTypeComboBox.SelectedItem).Content.ToString()!;
        MeetingStatus? statusFilter = reportType switch
        {
            "Scheduled" => MeetingStatus.Scheduled,
            "Cancelled" => MeetingStatus.Cancelled,
            "In Preparation" => MeetingStatus.InPreparation,
            _ => null
        };

        var meetings = _meetingService.GetMeetingsByCoordinator(_coordinatorId)
            .Where(m => {
                DateOnly meetingDate = m.ScheduledDate ?? m.DateRangeStart;
                return meetingDate >= startDate && meetingDate <= endDate;
            })
            .Where(m => statusFilter == null || m.Status == statusFilter)
            .ToList();

        MeetingsReportPdfExporter.Export(meetings, reportType, startDate, endDate);
    }
}
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs.Buildings.ResidentMeetings;

public partial class AttendanceDialog : Window
{
    public AttendanceDialog(List<ResidentMeetingAttendance> attendances)
    {
        InitializeComponent();

        if (attendances.Count == 0)
        {
            EmptyText.Visibility = Visibility.Visible;
            AttendanceList.Visibility = Visibility.Collapsed;
        }
        else
        {
            AttendanceList.ItemsSource = attendances
                .Select(a => $"Apartment {a.UnitNumber}")
                .ToList();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
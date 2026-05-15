using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class SelectDayDialog : Window
{
    private readonly long _requestId;
    private readonly CommonRoomRequestViewModel _viewModel;

    public SelectDayDialog(List<DateTime> freeDays, long requestId,
                           CommonRoomRequestViewModel viewModel)
    {
        InitializeComponent();
        _requestId = requestId;
        _viewModel = viewModel;
        DaysListBox.ItemsSource = freeDays;
    }

    private void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DaysListBox.SelectedItem is DateTime selectedDay)
        {
            _viewModel.ApproveWithDate(_requestId, selectedDay);
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Please select a day.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;

namespace CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;

public partial class CommonRoomRequestDialog : Window
{
    private readonly CommonRoomRequestDialogViewModel _viewModel;

    public CommonRoomRequestDialog(CommonRoomRequestDialogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    public DateTime? SelectedDateFrom => DateFromPicker.SelectedDate;
    public DateTime? SelectedDateTo => DateToPicker.SelectedDate;

    private void SendRequestButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.Validate(DateFromPicker.SelectedDate, DateToPicker.SelectedDate)) return;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.CommonRooms;
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
        TrySubmit();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        TrySetDialogResult(false);
        Close();
    }

    public void SetDateFromForDemo(DateTime? date)
    {
        DateFromPicker.SelectedDate = date;
    }

    public void SetDateToForDemo(DateTime? date)
    {
        DateToPicker.SelectedDate = date;
    }

    public void OpenDateFromPickerForDemo()
    {
        DateFromPicker.IsDropDownOpen = true;
    }

    public void CloseDateFromPickerForDemo()
    {
        DateFromPicker.IsDropDownOpen = false;
    }

    public void OpenDateToPickerForDemo()
    {
        DateToPicker.IsDropDownOpen = true;
    }

    public void CloseDateToPickerForDemo()
    {
        DateToPicker.IsDropDownOpen = false;
    }

    public bool ClickSendRequestForDemo()
    {
        return TrySubmit();
    }

    private bool TrySubmit()
    {
        if (!_viewModel.Validate(DateFromPicker.SelectedDate, DateToPicker.SelectedDate))
            return false;

        TrySetDialogResult(true);
        Close();
        return true;
    }

    private void TrySetDialogResult(bool result)
    {
        try
        {
            DialogResult = result;
        }
        catch (InvalidOperationException)
        {
            // Demo opens the dialog with Show(), so DialogResult cannot be set.
        }
    }
}
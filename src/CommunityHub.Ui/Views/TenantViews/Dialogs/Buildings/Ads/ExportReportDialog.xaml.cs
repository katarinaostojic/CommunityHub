using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using System.Windows;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class ExportReportDialog : Window
{
    public ExportReportDialog()
    {
        InitializeComponent();

        DateFromPicker.DisplayDateStart = DateTime.Today;
        DateToPicker.DisplayDateStart = DateTime.Today;
    }

    public DateOnly DateFrom { get; private set; }
    public DateOnly DateTo { get; private set; }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadPeriod())
            return;

        DialogResult = true;
        Close();
    }

    private bool TryReadPeriod()
    {
        if (DateFromPicker.SelectedDate == null || DateToPicker.SelectedDate == null)
        {
            ShowDateError("Please select a date range.");
            return false;
        }

        DateOnly dateFrom = DateOnly.FromDateTime(DateFromPicker.SelectedDate.Value);
        DateOnly dateTo = DateOnly.FromDateTime(DateToPicker.SelectedDate.Value);

        string? dateError = Ad.ValidateDateRange(dateFrom, dateTo);

        if (dateError != null)
        {
            ShowDateError(dateError);
            return false;
        }

        ClearDateError();

        DateFrom = dateFrom;
        DateTo = dateTo;
        return true;
    }

    private void ShowDateError(string error)
    {
        DateErrorTextBlock.Text = error;
        DateErrorTextBlock.Visibility = Visibility.Visible;
    }

    private void ClearDateError()
    {
        DateErrorTextBlock.Text = string.Empty;
        DateErrorTextBlock.Visibility = Visibility.Collapsed;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
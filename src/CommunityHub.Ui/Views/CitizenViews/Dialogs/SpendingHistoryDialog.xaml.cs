using System.Collections.Generic;
using System.Windows;
using CommunityHub.Application.DTOs.Neighborhoods.Budget;
using CommunityHub.Application.Services.Reports.Citizen;
using CommunityHub.Ui.Helpers.Citizen;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class SpendingHistoryDialog : Window
{
    private readonly List<ExpenseDto> _expenses;
    private readonly string _neighborhoodName;

    public SpendingHistoryDialog(BudgetViewModel viewModel, string neighborhoodName = "")
    {
        InitializeComponent();
        _expenses = viewModel.GetExpenses();
        _neighborhoodName = neighborhoodName;
        ExpensesList.ItemsSource = _expenses;
    }

    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ExpensesPdfExporter.Export(_expenses, _neighborhoodName);
            MsgHelper.Info("Msg_PdfSuccess", "Msg_PdfSuccessTitle");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"{MsgHelper.Get("Msg_PdfError", "Greška:")}\n{ex.Message}",
                MsgHelper.Get("Msg_Error", "Greška"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => Close();
}

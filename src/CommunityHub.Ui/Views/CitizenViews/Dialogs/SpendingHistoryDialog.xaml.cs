using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class SpendingHistoryDialog : Window
{
    public SpendingHistoryDialog(BudgetViewModel viewModel)
    {
        InitializeComponent();
        ExpensesList.ItemsSource = viewModel.GetExpenses();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => Close();
}
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods.Budget;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class AddExpensePage : Page
{
    private readonly AddExpenseViewModel _viewModel;

    public AddExpensePage(List<CategoryBudgetItemViewModel> categories, long neighborhoodId)
    {
        InitializeComponent();
        var donationService = Injector.CreateInstance<DonationService>();
        _viewModel = new AddExpenseViewModel(donationService, categories, neighborhoodId);
        DataContext = _viewModel;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Sakrij sve greške
        CategoryErrorText.Visibility = Visibility.Collapsed;
        AmountErrorText.Visibility = Visibility.Collapsed;
        DescriptionErrorText.Visibility = Visibility.Collapsed;
        DateErrorText.Visibility = Visibility.Collapsed;

        var errors = _viewModel.Validate();

        if (errors.ContainsKey("Category"))
        {
            CategoryErrorText.Text = errors["Category"];
            CategoryErrorText.Visibility = Visibility.Visible;
        }
        if (errors.ContainsKey("Amount"))
        {
            AmountErrorText.Text = errors["Amount"];
            AmountErrorText.Visibility = Visibility.Visible;
        }
        if (errors.ContainsKey("Description"))
        {
            DescriptionErrorText.Text = errors["Description"];
            DescriptionErrorText.Visibility = Visibility.Visible;
        }

        if (errors.Count > 0) return;

        string? error = _viewModel.Save();
        if (error != null)
        {
            AmountErrorText.Text = error;
            AmountErrorText.Visibility = Visibility.Visible;
            return;
        }

        CoordinatorMainWindow.Instance.GoBack();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.GoBack();
    }
}
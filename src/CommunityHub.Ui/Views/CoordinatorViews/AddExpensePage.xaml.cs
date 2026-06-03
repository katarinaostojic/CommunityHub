using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
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
        string? error = _viewModel.Save();
        if (error != null)
        {
            ErrorText.Text = error;
            ErrorBanner.Visibility = Visibility.Visible;
            return;
        }
        CoordinatorMainWindow.Instance.GoBack();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.GoBack();
    }
}
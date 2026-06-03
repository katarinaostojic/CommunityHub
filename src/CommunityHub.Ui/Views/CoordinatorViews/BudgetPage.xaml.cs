using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class BudgetPage : Page
{
    private readonly CoordinatorBudgetViewModel _viewModel;

    public BudgetPage(long coordinatorId)
    {
        InitializeComponent();
        var donationService = Injector.CreateInstance<DonationService>();
        var neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        _viewModel = new CoordinatorBudgetViewModel(donationService, neighborhoodService, coordinatorId);
        DataContext = _viewModel;
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        CategoryBudgetItemViewModel category = (CategoryBudgetItemViewModel)((Button)sender).Tag;
        if (_viewModel.SelectedNeighborhood == null) return;
        CoordinatorMainWindow.Instance.NavigateTo(
            new CategoryDetailsPage(category, _viewModel.SelectedNeighborhood.Id),
            category.CategoryName);
    }

    private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedNeighborhood == null) return;
        CoordinatorMainWindow.Instance.NavigateTo(
            new AddExpensePage(_viewModel.Categories.ToList(), _viewModel.SelectedNeighborhood.Id),
            "Add Expense");
    }
}
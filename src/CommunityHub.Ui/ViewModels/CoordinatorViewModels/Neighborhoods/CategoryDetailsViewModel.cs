using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CategoryDetailsViewModel : BaseViewModel
{
    private ObservableCollection<DonationItemViewModel> _donations = new();
    private ObservableCollection<ExpenseItemViewModel> _expenses = new();
    private bool _hasDonations;
    private bool _hasExpenses;

    public CategoryDetailsViewModel(DonationService donationService,
        CategoryBudgetItemViewModel category, long neighborhoodId)
    {
        CategoryName = category.CategoryName;
        TotalDisplay = $"Total: {category.BudgetDisplay}";

        var donations = donationService.GetDonationsByNeighborhood(neighborhoodId)
            .Where(d => d.CategoryName == category.CategoryName)
            .Select(d => new DonationItemViewModel(d))
            .ToList();
        Donations = new ObservableCollection<DonationItemViewModel>(donations);
        HasDonations = donations.Count > 0;

        var expenses = donationService.GetExpenses(neighborhoodId)
            .Where(e => e.CategoryName == category.CategoryName)
            .Select(e => new ExpenseItemViewModel(e))
            .ToList();
        Expenses = new ObservableCollection<ExpenseItemViewModel>(expenses);
        HasExpenses = expenses.Count > 0;
    }

    public string CategoryName { get; }
    public string TotalDisplay { get; }

    public ObservableCollection<DonationItemViewModel> Donations
    {
        get => _donations;
        private set => SetProperty(ref _donations, value);
    }

    public ObservableCollection<ExpenseItemViewModel> Expenses
    {
        get => _expenses;
        private set => SetProperty(ref _expenses, value);
    }

    public bool HasDonations
    {
        get => _hasDonations;
        private set => SetProperty(ref _hasDonations, value);
    }

    public bool HasExpenses
    {
        get => _hasExpenses;
        private set => SetProperty(ref _hasExpenses, value);
    }
}
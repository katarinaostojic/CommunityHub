using CommunityHub.Application.Services.Entities.Neighborhoods.Budget;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class AddExpenseViewModel : BaseViewModel
{
    private readonly DonationService _donationService;
    private readonly long _neighborhoodId;

    private CategoryBudgetItemViewModel? _selectedCategory;
    private string _amountText = string.Empty;
    private string _description = string.Empty;
    private DateTime _selectedDate = DateTime.Today;

    public AddExpenseViewModel(DonationService donationService,
        List<CategoryBudgetItemViewModel> categories, long neighborhoodId)
    {
        _donationService = donationService;
        _neighborhoodId = neighborhoodId;
        Categories = new ObservableCollection<CategoryBudgetItemViewModel>(categories);
        if (Categories.Count > 0)
            SelectedCategory = Categories[0];
    }

    public ObservableCollection<CategoryBudgetItemViewModel> Categories { get; }

    public CategoryBudgetItemViewModel? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public string AmountText
    {
        get => _amountText;
        set => SetProperty(ref _amountText, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }

    public string? Save()
    {
        if (SelectedCategory == null) return "Please select a category.";

        if (!decimal.TryParse(AmountText.Replace(",", "."),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal amount))
            return "Please enter a valid amount.";

        var (success, error) = _donationService.AddExpense(
            _neighborhoodId, SelectedCategory.CategoryId, amount, Description);

        return success ? null : error;
    }
}
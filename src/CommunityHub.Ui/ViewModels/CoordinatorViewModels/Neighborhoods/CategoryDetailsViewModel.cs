using CommunityHub.Application.Services.Entities.Neighborhoods;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CategoryDetailsViewModel : BaseViewModel
{
    private ObservableCollection<DonationItemViewModel> _donations = new();
    private ObservableCollection<ExpenseItemViewModel> _expenses = new();
    private bool _hasDonations;
    private bool _hasExpenses;
    private SeriesCollection _budgetSeriesCollection = new();

    public SeriesCollection BudgetSeriesCollection
    {
        get => _budgetSeriesCollection;
        private set => SetProperty(ref _budgetSeriesCollection, value);
    }

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

        decimal totalDonations = donationService.GetDonationsByNeighborhood(neighborhoodId)
            .Where(d => d.CategoryName == category.CategoryName)
            .Sum(d => d.Amount);
        decimal totalExpenses = donationService.GetExpenses(neighborhoodId)
            .Where(e => e.CategoryName == category.CategoryName)
            .Sum(e => e.Amount);

        BudgetSeriesCollection = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Donations",
                Values = new ChartValues<decimal> { totalDonations },
                Fill = new SolidColorBrush(Color.FromRgb(133, 212, 176)),
                StrokeThickness = 0,
                DataLabels = true,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80))
            },
            new ColumnSeries
            {
                Title = "Expenses",
                Values = new ChartValues<decimal> { totalExpenses },
                Fill = new SolidColorBrush(Color.FromRgb(244, 160, 181)),
                StrokeThickness = 0,
                DataLabels = true,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80))
            }
        };
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
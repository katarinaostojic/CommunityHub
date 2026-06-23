using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods.Budget;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Budget;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CategoryBudgetItemViewModel
{
    public long CategoryId { get; }
    public string CategoryName { get; }
    public decimal Budget { get; }
    public string BudgetDisplay { get; }

    public CategoryBudgetItemViewModel(CategoryBudgetDto dto)
    {
        CategoryId = dto.CategoryId;
        CategoryName = dto.CategoryName;
        Budget = dto.Budget;
        BudgetDisplay = $"{dto.Budget:F2} RSD";
    }
}

public class DonationItemViewModel
{
    public string AmountDisplay { get; }
    public string DateDisplay { get; }

    public DonationItemViewModel(DonationDto dto)
    {
        AmountDisplay = $"{dto.Amount:F2} RSD";
        DateDisplay = dto.CreatedAt;
    }
}

public class ExpenseItemViewModel
{
    public string CategoryName { get; }
    public string AmountDisplay { get; }
    public string Description { get; }
    public string DateDisplay { get; }

    public ExpenseItemViewModel(ExpenseDto dto)
    {
        CategoryName = dto.CategoryName;
        AmountDisplay = $"{dto.Amount:F2} RSD";
        Description = dto.Description;
        DateDisplay = dto.CreatedAt;
    }
}

public class CoordinatorBudgetViewModel : BaseViewModel
{
    private readonly DonationService _donationService;
    private readonly NeighborhoodService _neighborhoodService;
    private readonly long _coordinatorId;

    private ObservableCollection<NeighborhoodDto> _neighborhoods = new();
    private NeighborhoodDto? _selectedNeighborhood;
    private ObservableCollection<CategoryBudgetItemViewModel> _categories = new();
    private string _totalBudgetDisplay = "0.00 RSD";

    public CoordinatorBudgetViewModel(DonationService donationService,
    NeighborhoodService neighborhoodService, long coordinatorId, long? preselectedNeighborhoodId = null)
    {
        _donationService = donationService;
        _neighborhoodService = neighborhoodService;
        _coordinatorId = coordinatorId;
        LoadNeighborhoods(preselectedNeighborhoodId);
    }

    private void LoadNeighborhoods(long? preselectedNeighborhoodId = null)
    {
        var neighborhoods = _neighborhoodService.GetByCoordinator(_coordinatorId);
        Neighborhoods = new ObservableCollection<NeighborhoodDto>(neighborhoods);

        System.Diagnostics.Debug.WriteLine($"Preselected ID: {preselectedNeighborhoodId}");
        foreach (var n in Neighborhoods)
            System.Diagnostics.Debug.WriteLine($"Neighborhood: {n.Id} - {n.Name}");


        if (Neighborhoods.Count > 0)
        {
            SelectedNeighborhood = preselectedNeighborhoodId.HasValue
                ? Neighborhoods.FirstOrDefault(n => n.Id == preselectedNeighborhoodId.Value) ?? Neighborhoods[0]
                : Neighborhoods[0];
        }
    }

    public ObservableCollection<NeighborhoodDto> Neighborhoods
    {
        get => _neighborhoods;
        private set => SetProperty(ref _neighborhoods, value);
    }

    public NeighborhoodDto? SelectedNeighborhood
    {
        get => _selectedNeighborhood;
        set
        {
            SetProperty(ref _selectedNeighborhood, value);
            if (value != null) LoadBudget(value.Id);
        }
    }

    public ObservableCollection<CategoryBudgetItemViewModel> Categories
    {
        get => _categories;
        private set => SetProperty(ref _categories, value);
    }

    public string TotalBudgetDisplay
    {
        get => _totalBudgetDisplay;
        private set => SetProperty(ref _totalBudgetDisplay, value);
    }

   

    private void LoadBudget(long neighborhoodId)
    {
        var categories = _donationService.GetCategoryBudgets(neighborhoodId);
        Categories = new ObservableCollection<CategoryBudgetItemViewModel>(
            categories.Select(c => new CategoryBudgetItemViewModel(c)));

        decimal total = categories.Sum(c => c.Budget);
        TotalBudgetDisplay = $"{total:F2} RSD";
    }
}
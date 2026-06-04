using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class BudgetViewModel : BaseViewModel
{
    private readonly DonationService _service;
    private readonly long _neighborhoodId;
    private readonly long _citizenId;
    private ObservableCollection<CategoryBudgetDto> _categoryBudgets = new();
    private decimal _totalBudget;

    public BudgetViewModel(DonationService service, long neighborhoodId, long citizenId)
    {
        _service = service;
        _neighborhoodId = neighborhoodId;
        _citizenId = citizenId;
        LoadData();
    }

    public ObservableCollection<CategoryBudgetDto> CategoryBudgets
    {
        get => _categoryBudgets;
        private set => SetProperty(ref _categoryBudgets, value);
    }

    public decimal TotalBudget
    {
        get => _totalBudget;
        private set => SetProperty(ref _totalBudget, value);
    }

    public string TotalBudgetDisplay => $"{_totalBudget:0.##} RSD";

    public void LoadData()
    {
        var budgets = _service.GetCategoryBudgets(_neighborhoodId);
        CategoryBudgets = new ObservableCollection<CategoryBudgetDto>(budgets);
        TotalBudget = budgets.Sum(b => b.Budget);
        OnPropertyChanged(nameof(TotalBudgetDisplay));
    }

    public List<DonationCategory> GetCategories()
        => _service.GetCategories();

    public (bool success, string? error) Donate(long categoryId, decimal amount)
    {
        var result = _service.Donate(_citizenId, _neighborhoodId, categoryId, amount);
        if (result.success) LoadData();
        return result;
    }

    public List<ExpenseDto> GetExpenses()
        => _service.GetExpenses(_neighborhoodId);
}

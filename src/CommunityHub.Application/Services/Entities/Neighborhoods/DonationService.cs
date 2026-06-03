using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

public class DonationService
{
    private readonly IDonationRepository _repository;

    public DonationService(IDonationRepository repository)
    {
        _repository = repository;
    }

    public List<CategoryBudgetDto> GetCategoryBudgets(long neighborhoodId)
    {
        return _repository.GetCategories()
            .Select(c => new CategoryBudgetDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                Budget = _repository.GetBudgetByCategory(neighborhoodId, c.Id)
            }).ToList();
    }

    public List<string> GetCategoryNames()
        => _repository.GetCategories().Select(c => c.Name).ToList();

    public List<DonationCategory> GetCategories()
        => _repository.GetCategories();

    public (bool success, string? error) Donate(long citizenId, long neighborhoodId,
        long categoryId, decimal amount)
    {
        if (amount <= 0)
            return (false, "Iznos donacije mora biti veći od 0.");

        _repository.CreateDonation(citizenId, neighborhoodId, categoryId, amount);
        _repository.UpdateNeighborhoodBudget(neighborhoodId, amount);
        return (true, null);
    }

    public List<ExpenseDto> GetExpenses(long neighborhoodId)
    {
        return _repository.GetExpensesByNeighborhood(neighborhoodId)
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Amount = e.Amount,
                Description = e.Description,
                CreatedAt = e.CreatedAt.ToString("dd/MM/yyyy")
            }).ToList();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface IDonationRepository
{
    List<DonationCategory> GetCategories();
    void CreateDonation(long citizenId, long neighborhoodId, long categoryId, decimal amount);
    void UpdateNeighborhoodBudget(long neighborhoodId, decimal amount);
    List<Donation> GetDonationsByNeighborhood(long neighborhoodId);
    decimal GetBudgetByCategory(long neighborhoodId, long categoryId);
    List<Expense> GetExpensesByNeighborhood(long neighborhoodId);
}

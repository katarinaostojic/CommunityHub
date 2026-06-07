using System;
using System.Collections.Generic;
using System.Text;
namespace CommunityHub.Application.Domain.Entities.Neighborhoods.Budget;

public class Donation
{
    public long Id { get; private set; }
    public long CitizenId { get; private set; }
    public long NeighborhoodId { get; private set; }
    public long CategoryId { get; private set; }
    public string CategoryName { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly CreatedAt { get; private set; }

    public Donation(long id, long citizenId, long neighborhoodId, long categoryId,
        string categoryName, decimal amount, DateOnly createdAt)
    {
        Id = id;
        CitizenId = citizenId;
        NeighborhoodId = neighborhoodId;
        CategoryId = categoryId;
        CategoryName = categoryName;
        Amount = amount;
        CreatedAt = createdAt;
    }
}

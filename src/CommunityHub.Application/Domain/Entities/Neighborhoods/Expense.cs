using System;
using System.Collections.Generic;
using System.Text;
namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class Expense
{
    public long Id { get; private set; }
    public long NeighborhoodId { get; private set; }
    public long CategoryId { get; private set; }
    public string CategoryName { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public DateOnly CreatedAt { get; private set; }

    public Expense(long id, long neighborhoodId, long categoryId, string categoryName,
        decimal amount, string description, DateOnly createdAt)
    {
        Id = id;
        NeighborhoodId = neighborhoodId;
        CategoryId = categoryId;
        CategoryName = categoryName;
        Amount = amount;
        Description = description;
        CreatedAt = createdAt;
    }
}

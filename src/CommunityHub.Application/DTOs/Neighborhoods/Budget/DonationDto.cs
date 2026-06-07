using System;
using System.Collections.Generic;
using System.Text;
namespace CommunityHub.Application.DTOs.Neighborhoods.Budget;

public class DonationDto
{
    public long Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class ExpenseDto
{
    public long Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class CategoryBudgetDto
{
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
}

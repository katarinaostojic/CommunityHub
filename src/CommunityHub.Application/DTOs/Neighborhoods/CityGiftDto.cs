namespace CommunityHub.Application.DTOs.Neighborhoods;

public class CityGiftDto
{
    public long Id { get; init; }
    public decimal Amount { get; init; }
    public string AmountDisplay { get; init; }
    public string Deadline { get; init; }
    public bool IsAwarded { get; init; }
    public bool IsDeadlinePassed { get; init; }
    public bool HasApplied { get; init; }
    public bool CanApply { get; init; }
    public string? AwardedNeighborhoodName { get; init; }

    public CityGiftDto(long id, decimal amount, string deadline, bool isAwarded,
        bool isDeadlinePassed, bool hasApplied, bool canApply, string? awardedNeighborhoodName)
    {
        Id = id;
        Amount = amount;
        AmountDisplay = $"{amount:F2} RSD";
        Deadline = deadline;
        IsAwarded = isAwarded;
        IsDeadlinePassed = isDeadlinePassed;
        HasApplied = hasApplied;
        CanApply = canApply;
        AwardedNeighborhoodName = awardedNeighborhoodName;
    }

    public string StatusDisplay => IsAwarded
        ? $"✔ Awarded to {AwardedNeighborhoodName}"
        : IsDeadlinePassed ? "⏰ Deadline passed"
        : HasApplied ? "✔ Applied"
        : "Open for applications";
}
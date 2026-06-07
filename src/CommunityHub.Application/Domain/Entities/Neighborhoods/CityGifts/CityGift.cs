namespace CommunityHub.Application.Domain.Entities.Neighborhoods.CityGifts;

public class CityGift
{
    public long Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly Deadline { get; private set; }
    public long? AwardedNeighborhoodId { get; private set; }
    public bool IsAwarded { get; private set; }
    public DateOnly CreatedAt { get; private set; }
    public List<CityGiftApplication> Applications { get; private set; }

    public CityGift(long id, decimal amount, DateOnly deadline,
        long? awardedNeighborhoodId, bool isAwarded, DateOnly createdAt)
    {
        Id = id;
        Amount = amount;
        Deadline = deadline;
        AwardedNeighborhoodId = awardedNeighborhoodId;
        IsAwarded = isAwarded;
        CreatedAt = createdAt;
        Applications = new List<CityGiftApplication>();
    }

    public void SetApplications(List<CityGiftApplication> applications)
        => Applications = applications;

    public bool IsDeadlinePassed => DateOnly.FromDateTime(DateTime.Today) > Deadline;
    public bool CanApply(long neighborhoodId)
        => !IsAwarded && !IsDeadlinePassed && !Applications.Any(a => a.NeighborhoodId == neighborhoodId);
    public bool HasApplied(long neighborhoodId)
        => Applications.Any(a => a.NeighborhoodId == neighborhoodId);

    public long? DetermineWinner(List<(long NeighborhoodId, decimal Budget)> budgets)
    {
        if (!IsDeadlinePassed || IsAwarded) return null;
        if (!Applications.Any()) return null;

        var applicantIds = Applications.Select(a => a.NeighborhoodId).ToHashSet();
        var eligible = budgets.Where(b => applicantIds.Contains(b.NeighborhoodId)).ToList();

        if (!eligible.Any()) return null;

        return eligible.OrderBy(b => b.Budget).First().NeighborhoodId;
    }

    public void Award(long neighborhoodId)
    {
        AwardedNeighborhoodId = neighborhoodId;
        IsAwarded = true;
    }

    public decimal AmountPerCategory(int categoryCount)
        => categoryCount > 0 ? Amount / categoryCount : 0;

    public bool CanApply(long neighborhoodId, IEnumerable<long> coordinatorNeighborhoodIds)
    {
        if (IsAwarded || IsDeadlinePassed) return false;
        if (Applications.Any(a => a.NeighborhoodId == neighborhoodId)) return false;
        if (Applications.Any(a => coordinatorNeighborhoodIds.Contains(a.NeighborhoodId))) return false;
        return true;
    }
}
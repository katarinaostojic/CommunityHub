using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.Entities.Buildings.Ads;

public class Ad
{
    public long Id { get; private set; }
    public long BuildingId { get; private set; }
    public User Author { get; private set; }
    public AdType Type { get; private set; }
    public AdCategory Category { get; private set; }
    public string Description { get; private set; }
    public DateOnly DateFrom { get; private set; }
    public DateOnly DateTo { get; private set; }
    public AdStatus Status { get; private set; }
    public List<AdSlot> Slots { get; private set; }

    public Ad(
        long id,
        long buildingId,
        User author,
        AdType type,
        AdCategory category,
        string description,
        DateOnly dateFrom,
        DateOnly dateTo,
        AdStatus status)
    {
        Id = id;
        BuildingId = buildingId;
        Author = author;
        Type = type;
        Category = category;
        Description = description;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Status = status;
        Slots = new List<AdSlot>();
    }

    public Ad(
        long buildingId,
        User author,
        AdType type,
        AdCategory category,
        string description,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        Id = 0;
        BuildingId = buildingId;
        Author = author;
        Type = type;
        Category = category;
        Description = description;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Status = AdStatus.Active;
        Slots = new List<AdSlot>();
    }

    public void Archive()
    {
        Status = AdStatus.Archived;
    }

    public void Restore()
    {
        Status = AdStatus.Active;
    }

    public bool IsActive => AdRules.IsActive(Status);

    public bool IsExpired(DateOnly today)
    {
        return AdRules.IsExpired(Status, DateTo, today);
    }

    public bool OverlapsWith(DateOnly otherFrom, DateOnly otherTo)
    {
        return AdRules.OverlapsWith(DateFrom, DateTo, otherFrom, otherTo);
    }

    public AdType OppositeType => AdRules.GetOppositeType(Type);

    public bool IsEligibleMatchFor(Ad other)
    {
        return AdRules.IsEligibleMatchFor(this, other);
    }

    public static string? ValidateDescription(string description)
    {
        return AdRules.ValidateDescription(description);
    }

    public static string? ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return "Description is required.";

        return null;
    }

    public static string? ValidateDateRange(DateOnly dateFrom, DateOnly dateTo)
    {
        return AdRules.ValidateDateRange(dateFrom, dateTo);
    }
}
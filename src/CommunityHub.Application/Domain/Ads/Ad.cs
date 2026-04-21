using CommunityHub.Application.Domain.Ads;
using System.Net.NetworkInformation;

namespace CommunityHub.Application.Domain.Building;

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

    public Ad(long id, long buildingId, User author, AdType type, AdCategory category,
        string description, DateOnly dateFrom, DateOnly dateTo, AdStatus status)
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

    public void AddSlot(AdSlot slot) => Slots.Add(slot);

    public bool IsActive => Status == AdStatus.Active;

    public bool OverlapsWith(DateOnly otherFrom, DateOnly otherTo)
        => DateFrom <= otherTo && DateTo >= otherFrom;
}
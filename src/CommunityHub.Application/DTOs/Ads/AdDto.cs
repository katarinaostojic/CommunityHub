using CommunityHub.Application.Domain.Entities.Ads;

namespace CommunityHub.Application.DTOs.Ads;

public class AdDto
{
    public AdDto(
        long id,
        long buildingId,
        long authorId,
        string authorName,
        AdType type,
        AdCategory category,
        string description,
        DateOnly dateFrom,
        DateOnly dateTo,
        AdStatus status,
        List<AdSlotDto> slots)
    {
        Id = id;
        BuildingId = buildingId;
        AuthorId = authorId;
        AuthorName = authorName;
        Type = type;
        Category = category;
        Description = description;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Status = status;
        Slots = slots;
    }

    public long Id { get; init; }
    public long BuildingId { get; init; }
    public long AuthorId { get; init; }
    public string AuthorName { get; init; }
    public AdType Type { get; init; }
    public AdCategory Category { get; init; }
    public string Description { get; init; }
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public AdStatus Status { get; init; }
    public List<AdSlotDto> Slots { get; init; }

    public bool IsActive => Status == AdStatus.Active;

    public bool OverlapsWith(DateOnly otherFrom, DateOnly otherTo)
    {
        return DateFrom <= otherTo && DateTo >= otherFrom;
    }
}
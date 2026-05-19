using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Application.DTOs.Ads;

public class CreateAdDto
{
    public CreateAdDto(
        long buildingId,
        User author,
        AdType type,
        AdCategory category,
        string description,
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        BuildingId = buildingId;
        Author = author;
        Type = type;
        Category = category;
        Description = description;
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public long BuildingId { get; }
    public User Author { get; }
    public AdType Type { get; }
    public AdCategory Category { get; }
    public string Description { get; }
    public DateOnly DateFrom { get; }
    public DateOnly DateTo { get; }
}
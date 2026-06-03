namespace CommunityHub.Application.DTOs.Neighborhoods;

public class CityObjectDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int VoteCount { get; init; }

    public CityObjectDto(long id, string name, string description, int voteCount)
    {
        Id = id;
        Name = name;
        Description = description;
        VoteCount = voteCount;
    }

    public string VoteDisplay => VoteCount == 1 ? "1 vote" : $"{VoteCount} votes";
}

public class ReserveRequest
{
    public long CityObjectId { get; set; }
    public long NeighborhoodId { get; set; }
    public int DurationDays { get; set; }
    public DateOnly RangeFrom { get; set; }
    public DateOnly RangeTo { get; set; }

    public ReserveRequest(long cityObjectId, long neighborhoodId,
        int durationDays, DateOnly rangeFrom, DateOnly rangeTo)
    {
        CityObjectId = cityObjectId;
        NeighborhoodId = neighborhoodId;
        DurationDays = durationDays;
        RangeFrom = rangeFrom;
        RangeTo = rangeTo;
    }
}

public class SlotSuggestion
{
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public bool IsAlternative { get; init; }

    public SlotSuggestion(DateOnly dateFrom, DateOnly dateTo, bool isAlternative = false)
    {
        DateFrom = dateFrom;
        DateTo = dateTo;
        IsAlternative = isAlternative;
    }

    public string Display => $"{DateFrom:dd.MM.yyyy.} – {DateTo:dd.MM.yyyy.}";
    public string Label => IsAlternative ? $"⚠ Alternative: {Display}" : $"✔ {Display}";
}

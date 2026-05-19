namespace CommunityHub.Application.DTOs.Ads;

public class AdSlotDto
{
    public AdSlotDto(
        long id,
        long adId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        long? bookedByAdId)
    {
        Id = id;
        AdId = adId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        BookedByAdId = bookedByAdId;
    }

    public long Id { get; init; }
    public long AdId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public long? BookedByAdId { get; init; }

    public bool IsFree => BookedByAdId == null;
    public string TimeDisplay => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
}
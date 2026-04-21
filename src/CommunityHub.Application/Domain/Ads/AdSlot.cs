namespace CommunityHub.Application.Domain.Ads;

public class AdSlot
{
    public long Id { get; private set; }
    public long AdId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public long? BookedByAdId { get; private set; }

    public AdSlot(long id, long adId, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, long? bookedByAdId = null)
    {
        Id = id;
        AdId = adId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        BookedByAdId = bookedByAdId;
    }

    public bool IsFree => BookedByAdId == null;
}
namespace CommunityHub.Application.Domain.Entities.Buildings.Ads;

public class AdSlot
{
    private static readonly TimeOnly SlotStart = new(16, 0);
    private const int SlotsPerDay = 4;

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

    public static List<(DateOnly, TimeOnly, TimeOnly)> GenerateForDateRange(
        DateOnly dateFrom,
        DateOnly dateTo)
    {
        List<(DateOnly, TimeOnly, TimeOnly)> slots = new();

        for (DateOnly date = dateFrom; date <= dateTo; date = date.AddDays(1))
            AddDailySlots(slots, date);

        return slots;
    }

    private static void AddDailySlots(
        List<(DateOnly, TimeOnly, TimeOnly)> slots,
        DateOnly date)
    {
        for (int i = 0; i < SlotsPerDay; i++)
        {
            TimeOnly startTime = SlotStart.AddHours(i);
            TimeOnly endTime = SlotStart.AddHours(i + 1);
            slots.Add((date, startTime, endTime));
        }
    }
}
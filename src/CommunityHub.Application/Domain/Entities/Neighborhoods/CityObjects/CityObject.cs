namespace CommunityHub.Application.Domain.Entities.Neighborhoods.CityObjects;

public class CityObject
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int VoteCount { get; private set; }
    public List<CityObjectReservation> Reservations { get; private set; }
    public DateOnly? LastVisit { get; private set; }
    public bool HasVoted { get; private set; }

    public CityObject(long id, string name, string description, int voteCount,
    DateOnly? lastVisit = null, bool hasVoted = false)
    {
        Id = id;
        Name = name;
        Description = description;
        VoteCount = voteCount;
        LastVisit = lastVisit;
        HasVoted = hasVoted;
        Reservations = new List<CityObjectReservation>();
    }

    public void SetReservations(List<CityObjectReservation> reservations)
        => Reservations = reservations;

    public void Reserve(long neighborhoodId, DateOnly dateFrom, DateOnly dateTo)
    {
        Reservations.Add(new CityObjectReservation(0, Id, neighborhoodId, dateFrom, dateTo));
    }

    // Traži slobodan termin unutar zadanog opsega
    public DateOnlyRange? FindFreeSlotInRange(DateOnly rangeFrom, DateOnly rangeTo, int durationDays)
    {
        DateOnly candidate = rangeFrom;
        DateOnly candidateTo = candidate.AddDays(durationDays - 1);

        while (candidateTo <= rangeTo)
        {
            if (!HasOverlap(candidate, candidateTo))
                return new DateOnlyRange(candidate, candidateTo);

            candidate = candidate.AddDays(1);
            candidateTo = candidate.AddDays(durationDays - 1);
        }

        return null;
    }

    // Traži slobodne termine van zadanog opsega (alternativni prijedlozi)
    public List<DateOnlyRange> FindAlternativeSlots(DateOnly rangeFrom, DateOnly rangeTo,
        int durationDays, int maxSuggestions = 3)
    {
        List<DateOnlyRange> alternatives = new();

        // Traži unazad od rangeFrom
        DateOnly beforeTo = rangeFrom.AddDays(-1);
        DateOnly beforeFrom = beforeTo.AddDays(-(durationDays - 1));

        for (int i = 0; i < maxSuggestions * 3 && alternatives.Count < maxSuggestions; i++)
        {
            if (beforeFrom >= DateOnly.FromDateTime(DateTime.Today) && !HasOverlap(beforeFrom, beforeTo))
                alternatives.Add(new DateOnlyRange(beforeFrom, beforeTo));

            beforeTo = beforeFrom.AddDays(-1);
            beforeFrom = beforeTo.AddDays(-(durationDays - 1));
        }

        // Traži unaprijed od rangeTo
        DateOnly afterFrom = rangeTo.AddDays(1);
        DateOnly afterTo = afterFrom.AddDays(durationDays - 1);

        for (int i = 0; i < maxSuggestions * 3 && alternatives.Count < maxSuggestions * 2; i++)
        {
            if (!HasOverlap(afterFrom, afterTo))
                alternatives.Add(new DateOnlyRange(afterFrom, afterTo));

            afterFrom = afterTo.AddDays(1);
            afterTo = afterFrom.AddDays(durationDays - 1);
        }

        return alternatives.Take(maxSuggestions).ToList();
    }

    private bool HasOverlap(DateOnly from, DateOnly to)
        => Reservations.Any(r => r.OverlapsWith(from, to));
}

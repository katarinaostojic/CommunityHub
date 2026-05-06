namespace CommunityHub.Application.Domain;

public class TrustRecord
{
    public long CitizenId { get; private set; }
    public int EventsOrganized { get; private set; }
    public int EventsVolunteered { get; private set; }
    public DateOnly JoinedAt { get; private set; }

    public TrustRecord(long citizenId, int eventsOrganized, int eventsVolunteered, DateOnly joinedAt)
    {
        CitizenId = citizenId;
        EventsOrganized = eventsOrganized;
        EventsVolunteered = eventsVolunteered;
        JoinedAt = joinedAt;
    }

    public TrustLevel GetLevel()
    {
        int totalEvents = EventsOrganized + EventsVolunteered;
        bool moreThanOneYear = JoinedAt <= DateOnly.FromDateTime(DateTime.Now.AddYears(-1));

        if (!moreThanOneYear)
            return TrustLevel.New;

        if (totalEvents < 5)
            return TrustLevel.Inactive;

        if (totalEvents <= 10)
            return TrustLevel.Active;

        if (totalEvents <= 15)
            return TrustLevel.Distinguished;

        return TrustLevel.Trusted;
    }
}
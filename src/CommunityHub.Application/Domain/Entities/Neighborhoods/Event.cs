using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class Event
{
    public long Id { get; private set; }
    public long NeighborhoodId { get; private set; }
    public User Organizer { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateOnly EventDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public int MinVolunteers { get; private set; }
    public EventStatus Status { get; private set; }
    public List<EventItem> Items { get; private set; }
    public List<EventRegistration> Registrations { get; private set; }

    public Event(long id, long neighborhoodId, User organizer, string name, string description,
        DateOnly eventDate, TimeOnly startTime, int durationMinutes, int minVolunteers, EventStatus status)
    {
        Id = id;
        NeighborhoodId = neighborhoodId;
        Organizer = organizer;
        Name = name;
        Description = description;
        EventDate = eventDate;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        MinVolunteers = minVolunteers;
        Status = status;
        Items = new List<EventItem>();
        Registrations = new List<EventRegistration>();
    }

    public void AddItem(EventItem item) => Items.Add(item);
    public void AddRegistration(EventRegistration registration) => Registrations.Add(registration);

    public int VolunteerCount => Registrations.Count;
    public bool IsFinished => Status == EventStatus.Finished;
    public bool IsReadyToSchedule => HasMinVolunteers() && AllItemsTaken();
    public bool IsRegistered(long citizenId) => Registrations.Any(r => r.Citizen.Id == citizenId);
    public bool IsOrganizer(long citizenId) => Organizer.Id == citizenId;

    public void Schedule()
    {
        if (!IsReadyToSchedule) return;
        Status = EventStatus.Scheduled;
    }

    public void Cancel() => Status = EventStatus.Cancelled;
    public void Finish() => Status = EventStatus.Finished;

    public void CheckAndCancel(DateTime now)
    {
        if (Status != EventStatus.Preparation) return;
        if (EventStatusChecker.IsDeadlinePassed(GetEventDateTime(), now) && !IsReadyToSchedule)
            Cancel();
    }

    public void CheckAndFinish(DateTime now)
    {
        if (Status != EventStatus.Scheduled) return;
        if (EventStatusChecker.IsEventOver(GetEventDateTime(), DurationMinutes, now))
            Finish();
    }

    private bool HasMinVolunteers() => VolunteerCount >= MinVolunteers;
    private bool AllItemsTaken() => Items.Any() && Items.All(i => i.IsTaken);
    private DateTime GetEventDateTime() => EventDate.ToDateTime(StartTime);
}
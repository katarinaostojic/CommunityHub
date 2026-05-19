using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Application.Domain.Neighborhoods;

public class EventRegistration
{
    public long Id { get; private set; }
    public long EventId { get; private set; }
    public User Citizen { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public List<EventItem> SelectedItems { get; private set; }

    public EventRegistration(long id, long eventId, User citizen, DateTime registeredAt, bool? attended)
    {
        Id = id;
        EventId = eventId;
        Citizen = citizen;
        RegisteredAt = registeredAt;
        SelectedItems = new List<EventItem>();
        Attended = attended;
    }

    public void AddItem(EventItem item) => SelectedItems.Add(item);
    public bool? Attended { get; private set; }

    public void MarkAttended(bool attended) => Attended = attended;
}

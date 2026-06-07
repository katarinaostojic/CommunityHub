using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain.Entities.Neighborhoods.Events;

public class EventItem
{
    public long Id { get; private set; }
    public long EventId { get; private set; }
    public string Name { get; private set; }
    public bool IsTaken { get; private set; }

    public EventItem(long id, long eventId, string name, bool isTaken)
    {
        Id = id;
        EventId = eventId;
        Name = name;
        IsTaken = isTaken;
    }

    public void MarkAsTaken() => IsTaken = true;
}

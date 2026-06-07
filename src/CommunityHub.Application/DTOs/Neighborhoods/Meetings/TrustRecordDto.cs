using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities;

namespace CommunityHub.Application.DTOs.Neighborhoods.Meetings;

public class TrustRecordDto
{
    public long CitizenId { get; init; }
    public string CitizenFullName { get; init; }
    public int EventsOrganized { get; init; }
    public int EventsVolunteered { get; init; }
    public string JoinedAt { get; init; }
    public TrustLevel Level { get; init; }

    public TrustRecordDto(long citizenId, string citizenFullName, int eventsOrganized,
        int eventsVolunteered, string joinedAt, TrustLevel level)
    {
        CitizenId = citizenId;
        CitizenFullName = citizenFullName;
        EventsOrganized = eventsOrganized;
        EventsVolunteered = eventsVolunteered;
        JoinedAt = joinedAt;
        Level = level;
    }

    public int TotalEvents => EventsOrganized + EventsVolunteered;

    public string LevelDisplay => Level switch
    {
        TrustLevel.New => "🆕 Nov u kvartu",
        TrustLevel.Inactive => "😴 Neaktivan građanin",
        TrustLevel.Active => "✅ Aktivan građanin",
        TrustLevel.Distinguished => "⭐ Istaknut građanin",
        TrustLevel.Trusted => "🏆 Građanin od poverenja",
        _ => Level.ToString()
    };
}

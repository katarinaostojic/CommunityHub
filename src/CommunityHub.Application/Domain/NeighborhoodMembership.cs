using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain;

public class NeighborhoodMembership
{
    public long Id { get; private set; }
    public long CitizenId { get; private set; }
    public string CitizenName { get; private set; }
    public string CitizenSurname { get; private set; }
    public long NeighborhoodId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public NeighborhoodMembership(long id, long citizenId, string citizenName, string citizenSurname, long neighborhoodId, DateTime joinedAt)
    {
        Id = id;
        CitizenId = citizenId;
        CitizenName = citizenName;
        CitizenSurname = citizenSurname;
        NeighborhoodId = neighborhoodId;
        JoinedAt = joinedAt;
    }

    public string CitizenFullName => $"{CitizenName} {CitizenSurname}";
}

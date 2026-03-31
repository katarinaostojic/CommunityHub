using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain;

public class NeighborhoodAccessRequest
{
    public long Id { get; private set; }
    public long CitizenId { get; private set; }
    public string CitizenName { get; private set; }
    public string CitizenSurname { get; private set; }
    public long NeighborhoodId { get; private set; }
    public string NeighborhoodName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public NeighborhoodAccessRequest(long id, long citizenId, string citizenName, string citizenSurname,
        long neighborhoodId, string neighborhoodName, DateTime createdAt, string status, string? rejectionReason)
    {
        Id = id;
        CitizenId = citizenId;
        CitizenName = citizenName;
        CitizenSurname = citizenSurname;
        NeighborhoodId = neighborhoodId;
        NeighborhoodName = neighborhoodName;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public string CitizenFullName => $"{CitizenName} {CitizenSurname}";
}

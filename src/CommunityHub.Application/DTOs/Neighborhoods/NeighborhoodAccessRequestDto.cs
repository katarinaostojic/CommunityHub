using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Application.DTOs.Neighborhoods;

public class NeighborhoodAccessRequestDto
{
    public long Id { get; init; }
    public string CitizenName { get; init; }
    public string CitizenSurname { get; init; }
    public string? CitizenAddress { get; init; }
    public string NeighborhoodName { get; init; }
    public DateTime CreatedAt { get; init; }
    public RequestStatus Status { get; init; }
    public string? RejectionReason { get; init; }

    public NeighborhoodAccessRequestDto(
        long id, string citizenName, string citizenSurname,
        string? citizenAddress, string neighborhoodName,
        DateTime createdAt, RequestStatus status, string? rejectionReason = null)
    {
        Id = id;
        CitizenName = citizenName;
        CitizenSurname = citizenSurname;
        CitizenAddress = citizenAddress;
        NeighborhoodName = neighborhoodName;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public string CitizenFullName => $"{CitizenName} {CitizenSurname}";
    public string Address => CitizenAddress ?? "No address";
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy");
    public bool CanBeDeleted => Status == RequestStatus.PendingApproval;
    public bool HasRejectionReason => Status == RequestStatus.Rejected && RejectionReason != null;
}

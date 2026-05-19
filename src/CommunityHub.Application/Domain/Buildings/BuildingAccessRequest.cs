using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.Buildings;

public class BuildingAccessRequest
{
    public long Id { get; private set; }
    public User Tenant { get; private set; }
    public Building Building { get; private set; }
    public string UnitNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RequestStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public BuildingAccessRequest(long id, User tenant, Building building, string unitNumber,
                                  DateTime createdAt, RequestStatus status, string? rejectionReason = null)
    {
        Id = id;
        Tenant = tenant;
        Building = building;
        UnitNumber = unitNumber;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public BuildingAccessRequest(User tenant, Building building, string unitNumber)
    {
        Id = 0;
        Tenant = tenant;
        Building = building;
        UnitNumber = unitNumber;
        CreatedAt = DateTime.UtcNow;
        Status = RequestStatus.PendingApproval;
        RejectionReason = null;
    }

    public bool CanBeCancelled => Status == RequestStatus.PendingApproval;
    public bool HasRejectionReason => Status == RequestStatus.Rejected && RejectionReason != null;

    public void Approve()
    {
        Status = RequestStatus.Approved;
    }

    public void Reject(string? rejectionReason)
    {
        Status = RequestStatus.Rejected;
        RejectionReason = rejectionReason;
    }

    public void Cancel()
    {
        if (!CanBeCancelled)
            throw new InvalidOperationException("Only pending requests can be cancelled.");
    }
}
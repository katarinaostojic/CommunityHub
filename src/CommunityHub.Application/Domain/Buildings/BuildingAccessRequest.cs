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

    public BuildingAccessRequest(long id, User tenant, Building building, string unitNumber, DateTime createdAt, RequestStatus status, string? rejectionReason = null)
    {
        Id = id;
        Tenant = tenant;
        Building = building;
        UnitNumber = unitNumber;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }
}
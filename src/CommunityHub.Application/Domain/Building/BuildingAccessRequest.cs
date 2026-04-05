namespace CommunityHub.Application.Domain.Building;

public class BuildingAccessRequest
{
    public long Id { get; private set; }
    public User User { get; private set; }
    public Building Building { get; private set; }
    public string UnitNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RequestStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public BuildingAccessRequest(long id, User user, Building building, string unitNumber, DateTime createdAt, RequestStatus status, string? rejectionReason = null)
    {
        Id = id;
        User = user;
        Building = building;
        UnitNumber = unitNumber;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public string StatusDisplay => Status switch
    {
        RequestStatus.PendingApproval => "⏳ Pending approval",
        RequestStatus.Approved => "✔ Approved",
        RequestStatus.Rejected => "✕ Rejected",
        _ => Status.ToString()
    };

    public string RejectionReasonDisplay => RejectionReason != null
        ? $"Note: {RejectionReason}"
        : string.Empty;

    public bool CancelButtonVisible => Status == RequestStatus.PendingApproval;

    public bool RejectionReasonVisible => Status == RequestStatus.Rejected && RejectionReason != null;
}
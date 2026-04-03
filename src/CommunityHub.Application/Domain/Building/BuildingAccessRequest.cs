namespace CommunityHub.Application.Domain.Building;

public class BuildingAccessRequest
{
    public long Id { get; private set; }
    public User User { get; private set; }
    public Building Building { get; private set; }
    public string UnitNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public BuildingAccessRequest(long id, User user, Building building, string unitNumber, DateTime createdAt, string status, string? rejectionReason = null)
    {
        Id = id;
        User = user;
        Building = building;
        UnitNumber = unitNumber;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    //nije bas najbolje da stoji u domenskoj klasi

    public string StatusDisplay => Status switch
    {
        "pending approval" => "⏳ Pending approval",
        "accepted" => "✔ Accepted",
        "rejected" => "✕ Rejected",
        _ => Status
    };

    public string RejectionReasonDisplay => RejectionReason != null
        ? $"Note: {RejectionReason}"
        : string.Empty;

    public bool CancelButtonVisible => Status == "pending approval";

    public bool RejectionReasonVisible => Status == "rejected" && RejectionReason != null;
}
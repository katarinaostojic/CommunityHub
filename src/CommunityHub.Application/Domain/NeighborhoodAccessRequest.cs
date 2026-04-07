namespace CommunityHub.Application.Domain;

public class NeighborhoodAccessRequest
{
    public long Id { get; private set; }
    public User Citizen { get; private set; }
    public Neighborhood Neighborhood { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RequestStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public NeighborhoodAccessRequest(long id, User citizen, Neighborhood neighborhood,
        DateTime createdAt, RequestStatus status, string? rejectionReason)
    {
        Id = id;
        Citizen = citizen;
        Neighborhood = neighborhood;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }
    public void Approve()
    {
        if (Status != RequestStatus.PendingApproval)
            throw new InvalidOperationException("Only pending requests can be approved.");

        Status = RequestStatus.Approved;
    }

    public void Reject(string? reason)
    {
        if (Status != RequestStatus.PendingApproval)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        Status = RequestStatus.Rejected;
        RejectionReason = reason;
    }
}
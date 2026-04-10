using CommunityHub.Application.Domain;

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

    public NeighborhoodAccessRequest(User citizen, Neighborhood neighborhood)
    {
        Id = 0;
        Citizen = citizen;
        Neighborhood = neighborhood;
        CreatedAt = DateTime.Now;
        Status = RequestStatus.PendingApproval;
        RejectionReason = null;
    }
    public void Approve()
    {
        Status = RequestStatus.Approved;
    }

    public void Reject(string? reason)
    {
        Status = RequestStatus.Rejected;
        RejectionReason = reason;
    }
}
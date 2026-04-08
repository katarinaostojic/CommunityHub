using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Mappers;

public static class RequestStatusMapper
{
    public static RequestStatus Parse(string status) => status switch
    {
        "pending approval" => RequestStatus.PendingApproval,
        "accepted" => RequestStatus.Approved,
        "rejected" => RequestStatus.Rejected,
        _ => throw new ArgumentException($"Unknown request status: '{status}'")
    };
}
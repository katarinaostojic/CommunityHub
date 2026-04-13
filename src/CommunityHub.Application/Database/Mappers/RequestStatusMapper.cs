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

    public static string ToDbString(RequestStatus status) => status switch
    {
        RequestStatus.PendingApproval => "pending approval",
        RequestStatus.Approved => "accepted",
        RequestStatus.Rejected => "rejected",
        _ => throw new ArgumentException($"Unknown request status: '{status}'")
    };
}
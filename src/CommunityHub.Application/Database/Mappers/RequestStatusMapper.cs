using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Database.Mappers;

public static class RequestStatusMapper
{
    public static RequestStatus Parse(string status) => status switch
    {
        "pending approval" => RequestStatus.PendingApproval,
        "approved" => RequestStatus.Approved,
        "rejected" => RequestStatus.Rejected,
        _ => throw new ArgumentException($"Unknown request status: '{status}'")
    };

    public static string ToDbString(RequestStatus status) => status switch
    {
        RequestStatus.PendingApproval => "pending approval",
        RequestStatus.Approved => "approved",
        RequestStatus.Rejected => "rejected",
        _ => throw new ArgumentException($"Unknown request status: '{status}'")
    };
}
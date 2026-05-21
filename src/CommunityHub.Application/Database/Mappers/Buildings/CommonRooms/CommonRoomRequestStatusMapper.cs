using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

namespace CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;

public static class CommonRoomRequestStatusMapper
{
    public static string ToDatabaseValue(CommonRoomRequestStatus status)
    {
        return status switch
        {
            CommonRoomRequestStatus.Pending => "pending",
            CommonRoomRequestStatus.Approved => "approved",
            CommonRoomRequestStatus.Rejected => "rejected",
            CommonRoomRequestStatus.PendingDateChange => "pending_date_change",
            _ => throw new ArgumentException($"Unknown status: {status}")
        };
    }

    public static CommonRoomRequestStatus FromDatabaseValue(string status)
    {
        return status switch
        {
            "pending" => CommonRoomRequestStatus.Pending,
            "approved" => CommonRoomRequestStatus.Approved,
            "rejected" => CommonRoomRequestStatus.Rejected,
            "pending_date_change" => CommonRoomRequestStatus.PendingDateChange,
            _ => throw new ArgumentException($"Unknown status: {status}")
        };
    }
}
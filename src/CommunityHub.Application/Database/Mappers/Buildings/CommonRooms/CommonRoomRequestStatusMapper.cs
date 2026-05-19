using CommunityHub.Application.Domain.Buildings.CommonRooms;

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
}
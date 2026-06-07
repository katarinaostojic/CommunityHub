using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Database.Mappers.Buildings.ResidentMeetings;

public static class ResidentMeetingStatusMapper
{
    public static ResidentMeetingStatus FromDatabaseValue(string value) => value switch
    {
        "scheduled" => ResidentMeetingStatus.Scheduled,
        "confirmed" => ResidentMeetingStatus.Confirmed,
        "cancelled" => ResidentMeetingStatus.Cancelled,
        _ => throw new ArgumentException($"Unknown resident meeting status: '{value}'")
    };

    public static string ToDatabaseValue(ResidentMeetingStatus status) => status switch
    {
        ResidentMeetingStatus.Scheduled => "scheduled",
        ResidentMeetingStatus.Confirmed => "confirmed",
        ResidentMeetingStatus.Cancelled => "cancelled",
        _ => throw new ArgumentException($"Unknown resident meeting status: '{status}'")
    };
}
namespace CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

public class CreateResidentMeetingTopicSuggestionDto
{
    public long MeetingId { get; init; }
    public long TenantId { get; init; }
    public string Topic { get; init; }

    public CreateResidentMeetingTopicSuggestionDto(
        long meetingId,
        long tenantId,
        string topic)
    {
        MeetingId = meetingId;
        TenantId = tenantId;
        Topic = topic;
    }
}
namespace CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

public class ResidentMeetingTopicSuggestionDto
{
    public long Id { get; init; }
    public long MeetingId { get; init; }
    public long TenantId { get; init; }
    public string Topic { get; init; }
    public DateTime SuggestedAt { get; init; }

    public ResidentMeetingTopicSuggestionDto(
        long id,
        long meetingId,
        long tenantId,
        string topic,
        DateTime suggestedAt)
    {
        Id = id;
        MeetingId = meetingId;
        TenantId = tenantId;
        Topic = topic;
        SuggestedAt = suggestedAt;
    }
}
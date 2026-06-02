namespace CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingTopicSuggestion
{
    public long Id { get; private set; }
    public long MeetingId { get; private set; }
    public long TenantId { get; private set; }
    public string Topic { get; private set; }
    public DateTime SuggestedAt { get; private set; }

    public ResidentMeetingTopicSuggestion(
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

    public ResidentMeetingTopicSuggestion(long meetingId, long tenantId, string topic)
    {
        Id = 0;
        MeetingId = meetingId;
        TenantId = tenantId;
        Topic = topic.Trim();
        SuggestedAt = DateTime.Now;
    }
}
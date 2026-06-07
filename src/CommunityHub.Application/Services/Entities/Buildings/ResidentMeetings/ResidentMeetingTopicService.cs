using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingTopicService
{
    private readonly IResidentMeetingRepository _meetingRepository;
    private readonly ResidentMeetingAccessService _accessService;

    public ResidentMeetingTopicService(
        IResidentMeetingRepository meetingRepository,
        ResidentMeetingAccessService accessService)
    {
        _meetingRepository = meetingRepository;
        _accessService = accessService;
    }

    public void SuggestTopic(CreateResidentMeetingTopicSuggestionDto request, DateTime now)
    {
        ResidentMeeting meeting = _accessService.GetMeetingForTenant(
            request.MeetingId,
            request.TenantId);

        meeting.EnsureTopicCanBeSuggested(now);
        ValidateTopic(request.Topic);

        ResidentMeetingTopicSuggestion suggestion = new(
            request.MeetingId,
            request.TenantId,
            request.Topic);

        _meetingRepository.CreateTopicSuggestion(suggestion);
    }

    public List<ResidentMeetingTopicSuggestion> GetTopicSuggestions(long meetingId)
    {
        return _meetingRepository.GetTopicSuggestions(meetingId);
    }

    public void AddTopicFromSuggestion(long meetingId, string topic, DateTime now)
    {
        ResidentMeeting meeting = _meetingRepository.GetById(meetingId)
            ?? throw new InvalidOperationException("Residents' meeting was not found.");

        meeting.EnsureTopicsCanBeChanged(now);
        ValidateTopic(topic);

        _meetingRepository.AddTopic(meetingId, topic.Trim());
    }

    private static void ValidateTopic(string topic)
    {
        string? validationError = ResidentMeeting.ValidateTopic(topic);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }
}
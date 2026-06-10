using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Mappings.Buildings.ResidentMeetings;

public static class ResidentMeetingTopicSuggestionMappingExtensions
{
    public static ResidentMeetingTopicSuggestionDto ToDto(
        this ResidentMeetingTopicSuggestion suggestion)
    {
        return new ResidentMeetingTopicSuggestionDto(
            id: suggestion.Id,
            meetingId: suggestion.MeetingId,
            tenantId: suggestion.TenantId,
            topic: suggestion.Topic,
            suggestedAt: suggestion.SuggestedAt);
    }

    public static List<ResidentMeetingTopicSuggestionDto> ToDtoList(
        this IEnumerable<ResidentMeetingTopicSuggestion> suggestions)
    {
        return suggestions.Select(s => s.ToDto()).ToList();
    }
}
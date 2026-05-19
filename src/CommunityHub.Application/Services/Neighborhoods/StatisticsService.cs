using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Services;

public class StatisticsService
{
    private readonly TrustRecordDbRepository _repository;

    public StatisticsService(TrustRecordDbRepository repository)
    {
        _repository = repository;
    }

    public Dictionary<TrustLevel, int> GetTrustStatistics(long neighborhoodId)
    {
        return _repository.GetTrustLevelCounts(neighborhoodId);
    }

    public MeetingTheme? SuggestMeetingTheme(long neighborhoodId)
    {
        var stats = _repository.GetTrustLevelCounts(neighborhoodId);

        int newCount = stats.GetValueOrDefault(TrustLevel.New, 0);
        int inactiveCount = stats.GetValueOrDefault(TrustLevel.Inactive, 0);

        if (newCount == 0 && inactiveCount == 0)
            return null;

        if (newCount >= inactiveCount)
            return MeetingTheme.Welcome;

        return MeetingTheme.Motivation;
    }
}
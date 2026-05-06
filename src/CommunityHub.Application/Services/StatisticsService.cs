using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Services;

public class StatisticsService
{
    private readonly TrustRecordDbRepository _trustRepository;

    public StatisticsService()
    {
        _trustRepository = new TrustRecordDbRepository();
    }

    public Dictionary<TrustLevel, int> GetTrustStatistics(long neighborhoodId)
    {
        return _trustRepository.GetTrustLevelCounts(neighborhoodId);
    }

    public MeetingTheme? SuggestMeetingTheme(long neighborhoodId)
    {
        var stats = _trustRepository.GetTrustLevelCounts(neighborhoodId);

        int newCount = stats.GetValueOrDefault(TrustLevel.New, 0);
        int inactiveCount = stats.GetValueOrDefault(TrustLevel.Inactive, 0);

        if (newCount == 0 && inactiveCount == 0)
            return null;

        if (newCount >= inactiveCount)
            return MeetingTheme.Welcome;

        return MeetingTheme.Motivation;
    }
}
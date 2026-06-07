using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Meetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.Meetings;

namespace CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;

public class StatisticsService
{
    private readonly ITrustRecordRepository _repository;

    public StatisticsService(ITrustRecordRepository repository)
    {
        _repository = repository;
    }

    public Dictionary<TrustLevel, int> GetTrustStatistics(long neighborhoodId)
    {
        return _repository.GetTrustLevelCounts(neighborhoodId);
    }

    public MeetingTheme? SuggestMeetingTheme(long neighborhoodId)
    {
        var counts = _repository.GetTrustLevelCounts(neighborhoodId);
        var stats = new CitizenTrustStatistics(counts);
        return stats.SuggestTheme();
    }
}
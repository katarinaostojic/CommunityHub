using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Services;

public class StatisticsService
{
    private readonly TrustRecordDbRepository _repository;

    public StatisticsService(TrustRecordDbRepository repository)
    {
        _repository = repository;
    }

    public CitizenTrustStatistics GetTrustStatistics(long neighborhoodId)
    {
        var counts = _repository.GetTrustLevelCounts(neighborhoodId);
        return new CitizenTrustStatistics(counts);
    }

    public MeetingTheme? SuggestMeetingTheme(long neighborhoodId)
    {
        var statistics = GetTrustStatistics(neighborhoodId);
        return statistics.SuggestTheme();
    }
}
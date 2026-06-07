using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Reviews;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.Reviews;

public interface ICoordinatorReviewRepository
{
    void Create(long citizenId, long coordinatorId, long neighborhoodId, int rating, string? comment);
    List<CoordinatorReview> GetByNeighborhood(long neighborhoodId);
    CoordinatorReview? GetById(long reviewId);
    bool HasReviewedThisMonth(long citizenId, long coordinatorId);
    int GetRemovedReviewCount(long citizenId);
    void AddReport(long reviewId, long citizenId);
    bool HasAlreadyReported(long reviewId, long citizenId);
    void Remove(long reviewId);
    List<long> GetReporterIds(long reviewId);
}

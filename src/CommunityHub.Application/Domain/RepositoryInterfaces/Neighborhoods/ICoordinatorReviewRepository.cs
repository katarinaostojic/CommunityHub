using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

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
    int GetHighTrustReportCount(long reviewId);
}

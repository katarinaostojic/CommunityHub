using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

public class CoordinatorReviewService
{
    private readonly ICoordinatorReviewRepository _reviewRepository;
    private readonly ITrustRecordRepository _trustRecordRepository;

    public CoordinatorReviewService(
        ICoordinatorReviewRepository reviewRepository,
        ITrustRecordRepository trustRecordRepository)
    {
        _reviewRepository = reviewRepository;
        _trustRecordRepository = trustRecordRepository;
    }

    public List<CoordinatorReviewDto> GetByNeighborhood(long neighborhoodId, long currentCitizenId)
    {
        return _reviewRepository.GetByNeighborhood(neighborhoodId)
            .Select(r => new CoordinatorReviewDto
            {
                Id = r.Id,
                CitizenId = r.CitizenId,
                CitizenFullName = r.CitizenFullName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt.ToString("dd/MM/yyyy"),
                ReportCount = r.ReportCount,
                IsRemoved = r.IsRemoved,
                HasReported = _reviewRepository.HasAlreadyReported(r.Id, currentCitizenId),
                CanReport = r.CitizenId != currentCitizenId &&
                            !_reviewRepository.HasAlreadyReported(r.Id, currentCitizenId)
            }).ToList();
    }

    public (bool success, string? error) CreateReview(
        long citizenId, long coordinatorId, long neighborhoodId, int rating, string? comment)
    {
        if (_reviewRepository.GetRemovedReviewCount(citizenId) >= 3)
            return (false, "Ne možete ostavljati recenzije jer vam je uklonjeno više od 3 recenzije.");

        if (_reviewRepository.HasReviewedThisMonth(citizenId, coordinatorId))
            return (false, "Već ste ostavili recenziju ovog meseca.");

        if (rating < 3 && string.IsNullOrWhiteSpace(comment))
            return (false, "Za ocenu nižu od 3 morate ostaviti obrazloženje.");

        _reviewRepository.Create(citizenId, coordinatorId, neighborhoodId, rating, comment);
        return (true, null);
    }

    public (bool success, string? error) ReportReview(long reviewId, long citizenId)
    {
        if (_reviewRepository.HasAlreadyReported(reviewId, citizenId))
            return (false, "Već ste prijavili ovu recenziju.");

        _reviewRepository.AddReport(reviewId, citizenId);

        var review = _reviewRepository.GetById(reviewId);
        if (review == null) return (true, null);

        int highTrustCount = CountHighTrustReporters(reviewId, review.NeighborhoodId);

        if (review.ShouldBeRemoved(highTrustCount))
            _reviewRepository.Remove(reviewId);

        return (true, null);
    }

    private int CountHighTrustReporters(long reviewId, long neighborhoodId)
    {
        return _reviewRepository.GetReporterIds(reviewId).Count(id =>
        {
            try
            {
                TrustRecord tr = _trustRecordRepository.GetByCitizen(id, neighborhoodId);
                if (tr == null) return false;
                TrustLevel level = tr.GetLevel();
                return level == TrustLevel.Distinguished || level == TrustLevel.Trusted;
            }
            catch { return false; }
        });
    }
}
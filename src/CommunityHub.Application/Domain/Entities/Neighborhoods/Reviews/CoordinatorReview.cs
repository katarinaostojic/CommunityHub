using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain.Entities.Neighborhoods.Reviews;

public class CoordinatorReview
{
    public long Id { get; private set; }
    public long CitizenId { get; private set; }
    public long CoordinatorId { get; private set; }
    public long NeighborhoodId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateOnly CreatedAt { get; private set; }
    public int ReportCount { get; private set; }
    public bool IsRemoved { get; private set; }
    public string CitizenFullName { get; private set; }

    public CoordinatorReview(long id, long citizenId, long coordinatorId, long neighborhoodId,
        int rating, string? comment, DateOnly createdAt, int reportCount, bool isRemoved,
        string citizenFullName = "")
    {
        Id = id;
        CitizenId = citizenId;
        CoordinatorId = coordinatorId;
        NeighborhoodId = neighborhoodId;
        Rating = rating;
        Comment = comment;
        CreatedAt = createdAt;
        ReportCount = reportCount;
        IsRemoved = isRemoved;
        CitizenFullName = citizenFullName;
    }

    public bool ShouldBeRemoved(int highTrustReportCount)
    {
        if (ReportCount >= 10) return true;
        if (highTrustReportCount >= 5) return true;
        return false;
    }

    public void AddReport() => ReportCount++;
    public void Remove() => IsRemoved = true;
}
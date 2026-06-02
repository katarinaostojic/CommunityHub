using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;

public class ProblemReport
{
    public long Id { get; private set; }
    public long BuildingId { get; private set; }
    public User Tenant { get; private set; }
    public string Description { get; private set; }
    public ProblemPriority Priority { get; private set; }
    public DateTime ReportedAt { get; private set; }
    public ProblemReportStatus Status { get; private set; }

    public ProblemReport(
        long id,
        long buildingId,
        User tenant,
        string description,
        ProblemPriority priority,
        DateTime reportedAt,
        ProblemReportStatus status)
    {
        Id = id;
        BuildingId = buildingId;
        Tenant = tenant;
        Description = description;
        Priority = priority;
        ReportedAt = reportedAt;
        Status = status;
    }

    public ProblemReport(
        long buildingId,
        User tenant,
        string description,
        ProblemPriority priority)
    {
        Id = 0;
        BuildingId = buildingId;
        Tenant = tenant;
        Description = description.Trim();
        Priority = priority;
        ReportedAt = DateTime.Today;
        Status = ProblemReportStatus.Unresolved;
    }

    public bool CanConfirmResolved => Status == ProblemReportStatus.PotentiallySolved;

    public void MarkAsPotentiallySolved()
    {
        if (Status != ProblemReportStatus.Unresolved)
            throw new InvalidOperationException("Only unresolved reports can be marked as potentially solved.");

        Status = ProblemReportStatus.PotentiallySolved;
    }

    public void ConfirmResolved()
    {
        if (!CanConfirmResolved)
            throw new InvalidOperationException("Only potentially solved reports can be confirmed as solved.");

        Status = ProblemReportStatus.Solved;
    }

    public static string? ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return "Description is required.";

        if (description.Trim().Length < 10)
            return "Description must contain at least 10 characters.";

        if (description.Trim().Length > 1000)
            return "Description cannot be longer than 1000 characters.";

        return null;
    }
}
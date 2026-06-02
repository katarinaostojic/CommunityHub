using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;

namespace CommunityHub.Application.DTOs.Buildings.ProblemReports;

public class ProblemReportDto
{
    public long Id { get; init; }
    public long BuildingId { get; init; }
    public long TenantId { get; init; }
    public string TenantFullName { get; init; }
    public string Description { get; init; }
    public ProblemPriority Priority { get; init; }
    public DateTime ReportedAt { get; init; }
    public ProblemReportStatus Status { get; init; }

    public ProblemReportDto(
        long id,
        long buildingId,
        long tenantId,
        string tenantFullName,
        string description,
        ProblemPriority priority,
        DateTime reportedAt,
        ProblemReportStatus status)
    {
        Id = id;
        BuildingId = buildingId;
        TenantId = tenantId;
        TenantFullName = tenantFullName;
        Description = description;
        Priority = priority;
        ReportedAt = reportedAt;
        Status = status;
    }

    public bool CanConfirmResolved => Status == ProblemReportStatus.PotentiallySolved;
}
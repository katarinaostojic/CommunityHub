using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.DTOs.Buildings.ProblemReports;

namespace CommunityHub.Application.Mappings.Buildings.ProblemReports;

public static class ProblemReportMappingExtensions
{
    public static ProblemReportDto ToDto(this ProblemReport report)
    {
        return new ProblemReportDto(
            id: report.Id,
            buildingId: report.BuildingId,
            tenantId: report.Tenant.Id,
            tenantFullName: report.Tenant.FullName,
            description: report.Description,
            priority: report.Priority,
            reportedAt: report.ReportedAt,
            status: report.Status);
    }

    public static List<ProblemReportDto> ToDtoList(this IEnumerable<ProblemReport> reports)
    {
        return reports.Select(r => r.ToDto()).ToList();
    }
}
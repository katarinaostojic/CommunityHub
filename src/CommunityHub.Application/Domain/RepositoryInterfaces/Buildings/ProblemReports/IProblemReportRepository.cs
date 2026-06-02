using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ProblemReports;

public interface IProblemReportRepository
{
    List<ProblemReport> GetByTenantAndBuilding(long tenantId, long buildingId, ProblemReportStatus? status);
    int CountByTenantAndBuilding(long tenantId, long buildingId, ProblemReportStatus? status);
    ProblemReport? GetById(long reportId);
    long Create(ProblemReport report);
    void Update(ProblemReport report);
}
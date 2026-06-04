using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ProblemReports;
using CommunityHub.Application.DTOs.Buildings.ProblemReports;
using CommunityHub.Application.Mappings.Buildings.ProblemReports;

namespace CommunityHub.Application.Services.Entities.Buildings.ProblemReports;

public class ProblemReportService
{
    private readonly IProblemReportRepository _reportRepository;
    private readonly IBuildingMembershipRepository _membershipRepository;

    public ProblemReportService(
        IProblemReportRepository reportRepository,
        IBuildingMembershipRepository membershipRepository)
    {
        _reportRepository = reportRepository;
        _membershipRepository = membershipRepository;
    }

    public List<ProblemReportDto> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ProblemReportStatus? status = null)
    {
        EnsureTenantHasBuildingMembership(tenantId, buildingId);

        return _reportRepository
            .GetByTenantAndBuilding(tenantId, buildingId, status)
            .ToDtoList();
    }

    public int CountByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ProblemReportStatus? status = null)
    {
        EnsureTenantHasBuildingMembership(tenantId, buildingId);

        return _reportRepository.CountByTenantAndBuilding(tenantId, buildingId, status);
    }

    public List<ProblemReportDto> GetByBuilding(long buildingId)
    {
        return _reportRepository
            .GetByBuilding(buildingId)
            .ToDtoList();
    }

    public ProblemReportDto Create(CreateProblemReportDto request)
    {
        EnsureTenantHasBuildingMembership(request.Tenant.Id, request.BuildingId);
        ValidateProblemReport(request.Description);

        ProblemReport report = new ProblemReport(
            request.BuildingId,
            request.Tenant,
            request.Description,
            request.Priority);

        long reportId = _reportRepository.Create(report);
        return _reportRepository.GetById(reportId)!.ToDto();
    }

    public void ConfirmResolved(long reportId, long tenantId)
    {
        ProblemReport report = GetTenantReport(reportId, tenantId);
        report.ConfirmResolved();
        _reportRepository.Update(report);
    }

    public void MarkAsPotentiallySolved(long reportId)
    {
        ProblemReport report = GetReport(reportId);
        report.MarkAsPotentiallySolved();
        _reportRepository.Update(report);
    }

    private ProblemReport GetTenantReport(long reportId, long tenantId)
    {
        ProblemReport report = GetReport(reportId);

        report.EnsureReportedBy(tenantId);

        return report;
    }

    private ProblemReport GetReport(long reportId)
    {
        return _reportRepository.GetById(reportId)
            ?? throw new InvalidOperationException("Problem report was not found.");
    }

    private void EnsureTenantHasBuildingMembership(long tenantId, long buildingId)
    {
        bool hasMembership = _membershipRepository
            .GetByTenant(tenantId)
            .Any(m => m.Building.Id == buildingId);

        if (!hasMembership)
            throw new InvalidOperationException("Tenant is not a member of this building.");
    }

    private static void ValidateProblemReport(string description)
    {
        string? validationError = ProblemReport.ValidateDescription(description);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }
}
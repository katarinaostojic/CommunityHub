using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.DTOs.Buildings.ProblemReports;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ProblemReports;

public class ProblemReportRowViewModel
{
    private readonly ProblemReportDto _dto;

    public ProblemReportRowViewModel(ProblemReportDto dto, bool alternateRow)
    {
        _dto = dto;
        AlternateRow = alternateRow;
    }

    public long Id => _dto.Id;
    public string TenantFullName => _dto.TenantFullName;
    public string Description => _dto.Description;
    public DateTime ReportedAt => _dto.ReportedAt;
    public bool AlternateRow { get; }

    public string PriorityDisplay => _dto.Priority switch
    {
        ProblemPriority.CanWait => "Can wait",
        ProblemPriority.Soon => "Should be solved soon",
        ProblemPriority.Urgent => "Urgent",
        _ => _dto.Priority.ToString()
    };

    public string PriorityBackground => _dto.Priority switch
    {
        ProblemPriority.CanWait => "#D4EDDA",
        ProblemPriority.Soon => "#FFF3CD",
        ProblemPriority.Urgent => "#F8D7DA",
        _ => "#F0F0F0"
    };

    public string PriorityForeground => _dto.Priority switch
    {
        ProblemPriority.CanWait => "#155724",
        ProblemPriority.Soon => "#856404",
        ProblemPriority.Urgent => "#721C24",
        _ => "#2C3E50"
    };

    public string StatusDisplay => _dto.Status switch
    {
        ProblemReportStatus.Unresolved => "Unresolved",
        ProblemReportStatus.PotentiallySolved => "Potentially resolved",
        ProblemReportStatus.Solved => "Resolved",
        _ => _dto.Status.ToString()
    };

    public string StatusBackground => _dto.Status switch
    {
        ProblemReportStatus.Unresolved => "#F8D7DA",
        ProblemReportStatus.PotentiallySolved => "#FFF3CD",
        ProblemReportStatus.Solved => "#D4EDDA",
        _ => "#F0F0F0"
    };

    public string StatusForeground => _dto.Status switch
    {
        ProblemReportStatus.Unresolved => "#721C24",
        ProblemReportStatus.PotentiallySolved => "#856404",
        ProblemReportStatus.Solved => "#155724",
        _ => "#2C3E50"
    };

    // Manager može da označi kao potencijalno rešen samo ako je Unresolved
    public bool CanMarkResolved => _dto.Status == ProblemReportStatus.Unresolved;
}
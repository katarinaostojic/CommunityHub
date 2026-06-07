using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.DTOs.Buildings.ProblemReports;
using CommunityHub.Ui.Extensions.Buildings.ProblemReports;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ProblemReports;

public class ProblemReportRowViewModel : BaseViewModel
{
    private readonly ProblemReportDto _report;

    public ProblemReportRowViewModel(ProblemReportDto report)
    {
        _report = report;
    }

    public long Id => _report.Id;
    public string Description => _report.Description;
    public ProblemReportStatus Status => _report.Status;
    public string PriorityDisplay => _report.Priority.ToDisplayString();
    public string ReportedDateDisplay => _report.ReportedAt.ToString("dd.MM.yyyy.");

    public string StatusDisplay => _report.Status switch
    {
        ProblemReportStatus.Unresolved => "Unresolved",
        ProblemReportStatus.PotentiallySolved => "Potentially solved",
        ProblemReportStatus.Solved => "Solved",
        _ => _report.Status.ToString()
    };

    public bool CanConfirmResolved => _report.CanConfirmResolved;
}
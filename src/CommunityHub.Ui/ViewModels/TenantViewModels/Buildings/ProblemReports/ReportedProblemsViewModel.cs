using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Services.Entities.Buildings.ProblemReports;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ProblemReports;

public class ReportedProblemsViewModel : BaseViewModel
{
    private readonly ProblemReportService _problemReportService;
    private readonly long _tenantId;
    private readonly long _buildingId;

    private ObservableCollection<ProblemReportRowViewModel> _reports = new();
    private ProblemReportStatus? _currentFilter = null;
    private string _resultsCountText = string.Empty;
    private int _allCount;
    private int _unresolvedCount;
    private int _potentiallySolvedCount;
    private int _solvedCount;

    public ReportedProblemsViewModel(
        ProblemReportService problemReportService,
        long tenantId,
        long buildingId)
    {
        _problemReportService = problemReportService;
        _tenantId = tenantId;
        _buildingId = buildingId;
        Refresh();
    }

    public ObservableCollection<ProblemReportRowViewModel> Reports
    {
        get => _reports;
        private set => SetProperty(ref _reports, value);
    }

    public string ResultsCountText
    {
        get => _resultsCountText;
        private set => SetProperty(ref _resultsCountText, value);
    }

    public string AllButtonLabel => $"All ({_allCount})";
    public string UnresolvedButtonLabel => $"Unresolved ({_unresolvedCount})";
    public string PotentiallySolvedButtonLabel => $"Potentially solved ({_potentiallySolvedCount})";
    public string SolvedButtonLabel => $"Solved ({_solvedCount})";

    public void FilterAll() => ApplyFilter(null);

    public void FilterUnresolved() =>
        ApplyFilter(ProblemReportStatus.Unresolved);

    public void FilterPotentiallySolved() =>
        ApplyFilter(ProblemReportStatus.PotentiallySolved);

    public void FilterSolved() =>
        ApplyFilter(ProblemReportStatus.Solved);

    public void ConfirmResolved(long reportId)
    {
        _problemReportService.ConfirmResolved(reportId, _tenantId);
        Refresh();
    }

    public void Refresh()
    {
        List<ProblemReportRowViewModel> allReports = _problemReportService
            .GetByTenantAndBuilding(_tenantId, _buildingId)
            .Select(r => new ProblemReportRowViewModel(r))
            .ToList();

        UpdateCounts(allReports);
        UpdateReports(allReports);
    }

    private void ApplyFilter(ProblemReportStatus? status)
    {
        _currentFilter = status;
        Refresh();
    }

    private void UpdateReports(List<ProblemReportRowViewModel> allReports)
    {
        List<ProblemReportRowViewModel> filteredReports = allReports
            .Where(r => _currentFilter == null || r.Status == _currentFilter)
            .ToList();

        Reports = new ObservableCollection<ProblemReportRowViewModel>(filteredReports);
        ResultsCountText = $"Showing {filteredReports.Count} reported problem{(filteredReports.Count == 1 ? "" : "s")}";
    }

    private void UpdateCounts(List<ProblemReportRowViewModel> reports)
    {
        _allCount = reports.Count;
        _unresolvedCount = reports.Count(r => r.Status == ProblemReportStatus.Unresolved);
        _potentiallySolvedCount = reports.Count(r => r.Status == ProblemReportStatus.PotentiallySolved);
        _solvedCount = reports.Count(r => r.Status == ProblemReportStatus.Solved);

        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(UnresolvedButtonLabel));
        OnPropertyChanged(nameof(PotentiallySolvedButtonLabel));
        OnPropertyChanged(nameof(SolvedButtonLabel));
    }
}
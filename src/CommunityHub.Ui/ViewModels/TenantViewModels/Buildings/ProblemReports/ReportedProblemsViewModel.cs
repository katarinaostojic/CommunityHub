using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings.ProblemReports;
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

    public void CreateReport(User tenant, string description, ProblemPriority priority)
    {
        CreateProblemReportDto request = new CreateProblemReportDto(
            _buildingId,
            tenant,
            description,
            priority);

        _problemReportService.Create(request);
        Refresh();
    }

    public void ConfirmResolved(long reportId)
    {
        _problemReportService.ConfirmResolved(reportId, _tenantId);
        Refresh();
    }

    public void RejectResolution(long reportId)
    {
        _problemReportService.RejectResolution(reportId, _tenantId);
        Refresh();
    }

    public void Refresh()
    {
        UpdateCounts();
        UpdateReports();
    }

    private void ApplyFilter(ProblemReportStatus? status)
    {
        _currentFilter = status;
        Refresh();
    }

    private void UpdateReports()
    {
        List<ProblemReportRowViewModel> reports = _problemReportService
            .GetByTenantAndBuilding(_tenantId, _buildingId, _currentFilter)
            .Select(r => new ProblemReportRowViewModel(r))
            .ToList();

        Reports = new ObservableCollection<ProblemReportRowViewModel>(reports);
        ResultsCountText = $"Showing {reports.Count} reported problem{(reports.Count == 1 ? "" : "s")}";
    }

    private void UpdateCounts()
    {
        _allCount = _problemReportService.CountByTenantAndBuilding(_tenantId, _buildingId);

        _unresolvedCount = _problemReportService.CountByTenantAndBuilding(
            _tenantId,
            _buildingId,
            ProblemReportStatus.Unresolved);

        _potentiallySolvedCount = _problemReportService.CountByTenantAndBuilding(
            _tenantId,
            _buildingId,
            ProblemReportStatus.PotentiallySolved);

        _solvedCount = _problemReportService.CountByTenantAndBuilding(
            _tenantId,
            _buildingId,
            ProblemReportStatus.Solved);

        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(UnresolvedButtonLabel));
        OnPropertyChanged(nameof(PotentiallySolvedButtonLabel));
        OnPropertyChanged(nameof(SolvedButtonLabel));
    }
}
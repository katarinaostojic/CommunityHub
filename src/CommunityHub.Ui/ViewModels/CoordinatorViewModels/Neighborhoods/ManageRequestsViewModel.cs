using System.Collections.ObjectModel;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ManageRequestsViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequestService _requestService;
    private readonly long _coordinatorId;
    private readonly string? _neighborhoodName;

    private ObservableCollection<NeighborhoodAccessRequestCoordinatorViewModel> _requests = new();
    private RequestStatus? _currentFilter = null;
    private bool _sortDescending = true;
    private string _sortButtonLabel = "Sort by Date ↓";
    private SeriesCollection _requestStatusSeriesCollection = new();
    private int _pendingCount;
    private int _approvedCount;
    private int _rejectedCount;

    public ManageRequestsViewModel(NeighborhoodAccessRequestService requestService, long coordinatorId, string? neighborhoodName = null)
    {
        _requestService = requestService;
        _coordinatorId = coordinatorId;
        _neighborhoodName = neighborhoodName;
        LoadRequests();
    }

    public ObservableCollection<NeighborhoodAccessRequestCoordinatorViewModel> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    public SeriesCollection RequestStatusSeriesCollection
    {
        get => _requestStatusSeriesCollection;
        private set => SetProperty(ref _requestStatusSeriesCollection, value);
    }

    public int PendingCount
    {
        get => _pendingCount;
        private set => SetProperty(ref _pendingCount, value);
    }

    public int ApprovedCount
    {
        get => _approvedCount;
        private set => SetProperty(ref _approvedCount, value);
    }

    public int RejectedCount
    {
        get => _rejectedCount;
        private set => SetProperty(ref _rejectedCount, value);
    }

    public string[] RequestStatusLabels => new[] { "Pending", "Approved", "Rejected" };

    public string SortButtonLabel
    {
        get => _sortButtonLabel;
        private set => SetProperty(ref _sortButtonLabel, value);
    }

    public void FilterAll() { _currentFilter = null; LoadRequests(); }
    public void FilterPending() { _currentFilter = RequestStatus.PendingApproval; LoadRequests(); }
    public void FilterApproved() { _currentFilter = RequestStatus.Approved; LoadRequests(); }
    public void FilterRejected() { _currentFilter = RequestStatus.Rejected; LoadRequests(); }

    public void ToggleSort()
    {
        _sortDescending = !_sortDescending;
        SortButtonLabel = _sortDescending ? "Sort by Date ↓" : "Sort by Date ↑";
        LoadRequests();
    }

    public void Approve(NeighborhoodAccessRequestCoordinatorViewModel vm)
    {
        NeighborhoodAccessRequest? request = _requestService.GetById(vm.Id);
        if (request == null) throw new InvalidOperationException("Request not found.");
        _requestService.ApproveRequestWithMembership(request);
        LoadRequests();
    }

    public void Reject(NeighborhoodAccessRequestCoordinatorViewModel vm, string? reason)
    {
        NeighborhoodAccessRequest? request = _requestService.GetById(vm.Id);
        if (request == null) throw new InvalidOperationException("Request not found.");
        _requestService.RejectRequestForCoordinator(request, reason);
        LoadRequests();
    }

    private void LoadRequests()
    {
        string? statusFilter = _currentFilter == null ? null : StatusToString(_currentFilter.Value);

        var items = _requestService.GetAllByCoordinator(_coordinatorId, statusFilter, _sortDescending)
            .Where(r => _neighborhoodName == null || r.NeighborhoodName == _neighborhoodName)
            .Select(r => new NeighborhoodAccessRequestCoordinatorViewModel(r))
            .ToList();

        Requests = new ObservableCollection<NeighborhoodAccessRequestCoordinatorViewModel>(items);

        UpdateCounts();
        RequestStatusSeriesCollection = BuildChart();
    }

    private void UpdateCounts()
    {
        var all = _requestService.GetAllByCoordinator(_coordinatorId, null, true)
            .Where(r => _neighborhoodName == null || r.NeighborhoodName == _neighborhoodName)
            .ToList();

        PendingCount = all.Count(r => r.Status == RequestStatus.PendingApproval);
        ApprovedCount = all.Count(r => r.Status == RequestStatus.Approved);
        RejectedCount = all.Count(r => r.Status == RequestStatus.Rejected);
    }

    private SeriesCollection BuildChart() => new()
    {
        new ColumnSeries { Title = "Pending",  Values = new ChartValues<int> { PendingCount },  Fill = new SolidColorBrush(Color.FromRgb(247, 217, 106)), StrokeThickness = 0, DataLabels = true, FontSize = 11, Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)), ColumnPadding = 20 },
        new ColumnSeries { Title = "Approved", Values = new ChartValues<int> { ApprovedCount }, Fill = new SolidColorBrush(Color.FromRgb(133, 212, 176)), StrokeThickness = 0, DataLabels = true, FontSize = 11, Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)), ColumnPadding = 20 },
        new ColumnSeries { Title = "Rejected", Values = new ChartValues<int> { RejectedCount }, Fill = new SolidColorBrush(Color.FromRgb(244, 160, 181)), StrokeThickness = 0, DataLabels = true, FontSize = 11, Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)), ColumnPadding = 20 },
    };

    private static string StatusToString(RequestStatus status) => status switch
    {
        RequestStatus.PendingApproval => "pending approval",
        RequestStatus.Approved => "approved",
        RequestStatus.Rejected => "rejected",
        _ => throw new ArgumentException($"Unknown status: {status}")
    };
}
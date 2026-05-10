using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Buildings;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class MyBuildingRequestsViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private readonly long _tenantId;

    private ObservableCollection<BuildingAccessRequestViewModel> _requests = new();
    private RequestStatus? _currentFilter = null;
    private bool _sortDescending = true;
    private string _resultsCountText = string.Empty;
    private string _sortButtonLabel = "Sort by Date ↓";
    private int _allCount;
    private int _pendingCount;
    private int _approvedCount;
    private int _rejectedCount;

    public MyBuildingRequestsViewModel(BuildingAccessRequestService requestService, long tenantId)
    {
        _requestService = requestService;
        _tenantId = tenantId;
        LoadRequests();
        UpdateCounts();
    }

    public ObservableCollection<BuildingAccessRequestViewModel> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    public string ResultsCountText
    {
        get => _resultsCountText;
        private set => SetProperty(ref _resultsCountText, value);
    }

    public string SortButtonLabel
    {
        get => _sortButtonLabel;
        private set => SetProperty(ref _sortButtonLabel, value);
    }

    public string AllButtonLabel => $"All ({_allCount})";
    public string PendingButtonLabel => $"Pending approval ({_pendingCount})";
    public string ApprovedButtonLabel => $"Approved ({_approvedCount})";
    public string RejectedButtonLabel => $"Rejected ({_rejectedCount})";

    public void FilterAll()
    {
        _currentFilter = null;
        LoadRequests();
    }

    public void FilterPending()
    {
        _currentFilter = RequestStatus.PendingApproval;
        LoadRequests();
    }

    public void FilterApproved()
    {
        _currentFilter = RequestStatus.Approved;
        LoadRequests();
    }

    public void FilterRejected()
    {
        _currentFilter = RequestStatus.Rejected;
        LoadRequests();
    }

    public void ToggleSort()
    {
        _sortDescending = !_sortDescending;
        SortButtonLabel = _sortDescending ? "Sort by Date ↓" : "Sort by Date ↑";
        LoadRequests();
    }

    public void CancelRequest(long requestId)
    {
        _requestService.Delete(requestId);
        LoadRequests();
        UpdateCounts();
    }

    private void LoadRequests()
    {
        var items = _requestService.GetAllByTenant(_tenantId, _currentFilter, _sortDescending)
            .Select(r => new BuildingAccessRequestViewModel(r))
            .ToList();

        Requests = new ObservableCollection<BuildingAccessRequestViewModel>(items);
        ResultsCountText = $"Showing {items.Count} results";
    }

    private void UpdateCounts()
    {
        _allCount = _requestService.CountByTenantAndStatus(_tenantId, null);
        _pendingCount = _requestService.CountByTenantAndStatus(_tenantId, RequestStatus.PendingApproval);
        _approvedCount = _requestService.CountByTenantAndStatus(_tenantId, RequestStatus.Approved);
        _rejectedCount = _requestService.CountByTenantAndStatus(_tenantId, RequestStatus.Rejected);

        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(PendingButtonLabel));
        OnPropertyChanged(nameof(ApprovedButtonLabel));
        OnPropertyChanged(nameof(RejectedButtonLabel));
    }
}
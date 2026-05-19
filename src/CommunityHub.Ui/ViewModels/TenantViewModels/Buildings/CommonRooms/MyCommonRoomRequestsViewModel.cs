using CommunityHub.Application.Domain.Buildings.CommonRooms;
using CommunityHub.Application.Services.Buildings.CommonRooms;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;

public class MyCommonRoomRequestsViewModel : BaseViewModel
{
    private readonly CommonRoomRequestService _requestService;
    private readonly long _tenantId;
    private readonly long _buildingId;

    private ObservableCollection<CommonRoomRequestRowViewModel> _requests = new();
    private CommonRoomRequestStatus? _currentFilter = null;
    private string _resultsCountText = string.Empty;
    private int _allCount;
    private int _pendingCount;
    private int _dateChangeCount;
    private int _approvedCount;
    private int _rejectedCount;

    public MyCommonRoomRequestsViewModel(CommonRoomRequestService requestService, long tenantId, long buildingId)
    {
        _requestService = requestService;
        _tenantId = tenantId;
        _buildingId = buildingId;
        LoadRequests();
        UpdateCounts();
    }

    public ObservableCollection<CommonRoomRequestRowViewModel> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    public string ResultsCountText
    {
        get => _resultsCountText;
        private set => SetProperty(ref _resultsCountText, value);
    }

    public string AllButtonLabel => $"All ({_allCount})";
    public string PendingButtonLabel => $"Pending approval ({_pendingCount})";
    public string DateChangeButtonLabel => $"Date change ({_dateChangeCount})";
    public string ApprovedButtonLabel => $"Approved ({_approvedCount})";
    public string RejectedButtonLabel => $"Rejected ({_rejectedCount})";

    public void FilterAll() => ApplyFilter(null);

    public void FilterPending() =>
        ApplyFilter(CommonRoomRequestStatus.Pending);

    public void FilterDateChange() =>
        ApplyFilter(CommonRoomRequestStatus.PendingDateChange);

    public void FilterApproved() =>
        ApplyFilter(CommonRoomRequestStatus.Approved);

    public void FilterRejected() =>
        ApplyFilter(CommonRoomRequestStatus.Rejected);

    private void ApplyFilter(CommonRoomRequestStatus? status)
    {
        _currentFilter = status;
        LoadRequests();
    }

    public void CancelRequest(long requestId)
    {
        _requestService.CancelRequest(requestId);
        LoadRequests();
        UpdateCounts();
    }

    public void AcceptDateChange(long requestId)
    {
        _requestService.AcceptProposedDateChange(requestId);
        LoadRequests();
        UpdateCounts();
    }

    private void LoadRequests()
    {
        var items = _requestService.GetByTenantAndBuilding(_tenantId, _buildingId)
                    .Where(r => _currentFilter == null || r.Status == _currentFilter)
                    .Select(r => new CommonRoomRequestRowViewModel(r))
                    .ToList();

        Requests = new ObservableCollection<CommonRoomRequestRowViewModel>(items);
        ResultsCountText = $"Showing {items.Count} result{(items.Count == 1 ? "" : "s")}";
    }

    private void UpdateCounts()
    {
        var all = _requestService.GetByTenantAndBuilding(_tenantId, _buildingId);
        _allCount = all.Count;
        _pendingCount = all.Count(r => r.Status == CommonRoomRequestStatus.Pending);
        _dateChangeCount = all.Count(r => r.Status == CommonRoomRequestStatus.PendingDateChange);
        _approvedCount = all.Count(r => r.Status == CommonRoomRequestStatus.Approved);
        _rejectedCount = all.Count(r => r.Status == CommonRoomRequestStatus.Rejected);

        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(PendingButtonLabel));
        OnPropertyChanged(nameof(DateChangeButtonLabel));
        OnPropertyChanged(nameof(ApprovedButtonLabel));
        OnPropertyChanged(nameof(RejectedButtonLabel));
    }
}
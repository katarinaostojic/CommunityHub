using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;
using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ManageRequestsViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequestService _requestService;
    private readonly long _coordinatorId;

    private ObservableCollection<NeighborhoodAccessRequestCoordinatorViewModel> _requests = new();
    private RequestStatus? _currentFilter = null;
    private bool _sortDescending = true;
    private string _sortButtonLabel = "Sort by Date ↓";

    public ManageRequestsViewModel(NeighborhoodAccessRequestService requestService, long coordinatorId)
    {
        _requestService = requestService;
        _coordinatorId = coordinatorId;
        LoadRequests();
    }

    public ObservableCollection<NeighborhoodAccessRequestCoordinatorViewModel> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    public string SortButtonLabel
    {
        get => _sortButtonLabel;
        private set => SetProperty(ref _sortButtonLabel, value);
    }

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
            .Select(r => new NeighborhoodAccessRequestCoordinatorViewModel(r))
            .ToList();
        Requests = new ObservableCollection<NeighborhoodAccessRequestCoordinatorViewModel>(items);
    }

    private string StatusToString(RequestStatus status) => status switch
    {
        RequestStatus.PendingApproval => "pending approval",
        RequestStatus.Approved => "accepted",
        RequestStatus.Rejected => "rejected",
        _ => throw new ArgumentException($"Unknown status: {status}")
    };
}
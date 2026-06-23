using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;
using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;

public class AccessRequestsViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private readonly long _managerId;

    private ObservableCollection<BuildingAccessRequestDto> _requests = new();
    public ObservableCollection<BuildingAccessRequestDto> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    private ObservableCollection<AccessRequestRowViewModel> _rowViewModels = new();
    public ObservableCollection<AccessRequestRowViewModel> RowViewModels
    {
        get => _rowViewModels;
        private set => SetProperty(ref _rowViewModels, value);
    }

    private bool _sortDescending = true;
    public bool SortDescending
    {
        get => _sortDescending;
        private set => SetProperty(ref _sortDescending, value);
    }

    private string? _currentStatusFilter = null;

    public string AllButtonLabel => $"All ({CountByStatus(null)})";
    public string PendingButtonLabel => $"Pending ({CountByStatus(RequestStatus.PendingApproval)})";
    public string ApprovedButtonLabel => $"Approved ({CountByStatus(RequestStatus.Approved)})";
    public string RejectedButtonLabel => $"Rejected ({CountByStatus(RequestStatus.Rejected)})";

    public AccessRequestsViewModel(long managerId)
    {
        _managerId = managerId;
        _requestService = Injector.CreateInstance<BuildingAccessRequestService>();
        LoadRequests();
    }

    public void LoadRequests()
    {
        List<BuildingAccessRequestDto> requests = _requestService.GetAllByManager(
            _managerId, _currentStatusFilter, _sortDescending);
        Requests = new ObservableCollection<BuildingAccessRequestDto>(requests);
        RowViewModels = new ObservableCollection<AccessRequestRowViewModel>(
            requests.Select((r, i) => new AccessRequestRowViewModel(r, i % 2 == 1)).ToList()
        );
        RefreshButtonLabels();
    }

    public void SetStatusFilter(string? status)
    {
        _currentStatusFilter = status;
        LoadRequests();
    }

    public void ToggleSort()
    {
        SortDescending = !_sortDescending;
        LoadRequests();
    }

    public void ApproveRequest(long requestId)
    {
        BuildingAccessRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        _requestService.ApproveRequest(request);
        LoadRequests();
    }

    public void RejectRequest(long requestId, string? rejectionReason)
    {
        BuildingAccessRequest? request = _requestService.GetById(requestId);
        if (request == null) return;
        _requestService.RejectRequest(request, rejectionReason);
        LoadRequests();
    }

    private int CountByStatus(RequestStatus? status)
    {
        List<BuildingAccessRequestDto> all = _requestService.GetAllByManager(_managerId, null, _sortDescending);
        return status == null ? all.Count : all.Count(r => r.Status == status);
    }

    private void RefreshButtonLabels()
    {
        OnPropertyChanged(nameof(AllButtonLabel));
        OnPropertyChanged(nameof(PendingButtonLabel));
        OnPropertyChanged(nameof(ApprovedButtonLabel));
        OnPropertyChanged(nameof(RejectedButtonLabel));
    }
}
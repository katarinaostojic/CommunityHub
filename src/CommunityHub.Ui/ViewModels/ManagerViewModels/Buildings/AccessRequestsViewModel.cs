using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;
using CommunityHub.Application.Domain.Entities.Buildings;

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
}
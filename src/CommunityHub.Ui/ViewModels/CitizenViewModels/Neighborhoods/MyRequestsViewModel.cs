using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class MyRequestsViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequestService _service;
    private readonly long _citizenId;

    private ObservableCollection<NeighborhoodAccessRequestViewModel> _requests = new();
    private string _resultsText = string.Empty;
    private string _sortButtonLabel = "Date ↓";
    private bool _sortDescending = true;
    private RequestStatus? _currentFilter = null;

    public MyRequestsViewModel(NeighborhoodAccessRequestService service, long citizenId)
    {
        _service = service;
        _citizenId = citizenId;
        LoadRequests();
    }

    public ObservableCollection<NeighborhoodAccessRequestViewModel> Requests
    {
        get => _requests;
        private set => SetProperty(ref _requests, value);
    }

    public string ResultsText
    {
        get => _resultsText;
        private set => SetProperty(ref _resultsText, value);
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
        SortButtonLabel = _sortDescending ? "Date ↓" : "Date ↑";
        LoadRequests();
    }

    public void DeleteRequest(long requestId)
    {
        _service.Delete(requestId);
        LoadRequests();
    }

    private void LoadRequests()
    {
        string? statusFilter = _currentFilter switch
        {
            RequestStatus.PendingApproval => "pending approval",
            RequestStatus.Approved => "approved",
            RequestStatus.Rejected => "rejected",
            _ => null
        };

        var items = _service.GetAllByCitizen(_citizenId, statusFilter, _sortDescending)
            .Select(r => new NeighborhoodAccessRequestViewModel(r))
            .ToList();

        Requests = new ObservableCollection<NeighborhoodAccessRequestViewModel>(items);
        string label = System.Windows.Application.Current.Resources["Requests_Title"]?.ToString()
               ?? "Requests";
        ResultsText = $"{label}: {items.Count}";
    }
}
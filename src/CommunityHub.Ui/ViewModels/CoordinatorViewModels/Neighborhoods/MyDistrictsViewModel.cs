using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels;

public class DistrictCardViewModel
{
    public NeighborhoodDto District { get; }
    public int PendingCount { get; }
    public bool HasPending => PendingCount > 0;
    public string PendingText => PendingCount > 0 ? $"{PendingCount} requests pending" : string.Empty;

    public DistrictCardViewModel(NeighborhoodDto district, int pendingCount)
    {
        District = district;
        PendingCount = pendingCount;
    }
}

public class MyDistrictsViewModel : BaseViewModel
{
    private readonly NeighborhoodService _neighborhoodService;
    private readonly NeighborhoodAccessRequestService _requestService;
    private readonly long _coordinatorId;
    public long CoordinatorId => _coordinatorId;

    private ObservableCollection<DistrictCardViewModel> _districts = new();

    public MyDistrictsViewModel(NeighborhoodService neighborhoodService, NeighborhoodAccessRequestService requestService, long coordinatorId)
    {
        _neighborhoodService = neighborhoodService;
        _requestService = requestService;
        _coordinatorId = coordinatorId;
        LoadDistricts();
    }

    public ObservableCollection<DistrictCardViewModel> Districts
    {
        get => _districts;
        private set => SetProperty(ref _districts, value);
    }

    public void LoadDistricts()
    {
        var districts = _neighborhoodService.GetByCoordinator(_coordinatorId);
        var cards = districts.Select(d => new DistrictCardViewModel(
            d,
            _requestService.CountPendingByNeighborhood(d.Id)
        )).ToList();
        Districts = new ObservableCollection<DistrictCardViewModel>(cards);
    }
}
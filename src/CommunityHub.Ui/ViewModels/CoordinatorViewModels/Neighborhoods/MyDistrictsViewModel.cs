using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels;

public class MyDistrictsViewModel : BaseViewModel
{
    private readonly NeighborhoodService _neighborhoodService;
    private readonly long _coordinatorId;
    public long CoordinatorId => _coordinatorId;

    private ObservableCollection<NeighborhoodDto> _districts = new();

    public MyDistrictsViewModel(NeighborhoodService neighborhoodService, long coordinatorId)
    {
        _neighborhoodService = neighborhoodService;
        _coordinatorId = coordinatorId;
        LoadDistricts();
    }

    public ObservableCollection<NeighborhoodDto> Districts
    {
        get => _districts;
        private set => SetProperty(ref _districts, value);
    }

    public void LoadDistricts()
    {
        var districts = _neighborhoodService.GetByCoordinator(_coordinatorId);
        Districts = new ObservableCollection<NeighborhoodDto>(districts);
    }
}
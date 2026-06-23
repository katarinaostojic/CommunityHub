using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CityObjectsViewModel : BaseViewModel
{
    private readonly CityObjectService _cityObjectService;
    private readonly NeighborhoodService _neighborhoodService;
    private readonly long _coordinatorId;

    private ObservableCollection<NeighborhoodDto> _neighborhoods = new();
    private NeighborhoodDto? _selectedNeighborhood;
    private ObservableCollection<CityObjectItemViewModel> _cityObjects = new();

    public CityObjectsViewModel(CityObjectService cityObjectService,
     NeighborhoodService neighborhoodService, long coordinatorId, long? preselectedNeighborhoodId = null)
    {
        _cityObjectService = cityObjectService;
        _neighborhoodService = neighborhoodService;
        _coordinatorId = coordinatorId;
        LoadNeighborhoods(preselectedNeighborhoodId);

    }

    public ObservableCollection<NeighborhoodDto> Neighborhoods
    {
        get => _neighborhoods;
        private set => SetProperty(ref _neighborhoods, value);
    }

    public NeighborhoodDto? SelectedNeighborhood
    {
        get => _selectedNeighborhood;
        set
        {
            SetProperty(ref _selectedNeighborhood, value);
            LoadCityObjects();
        }
    }

    public ObservableCollection<CityObjectItemViewModel> CityObjects
    {
        get => _cityObjects;
        private set => SetProperty(ref _cityObjects, value);
    }

    public long? CurrentNeighborhoodId => _selectedNeighborhood?.Id;

    private void LoadNeighborhoods(long? preselectedNeighborhoodId = null)
    {
        var neighborhoods = _neighborhoodService.GetByCoordinator(_coordinatorId);
        Neighborhoods = new ObservableCollection<NeighborhoodDto>(neighborhoods);
        if (Neighborhoods.Count > 0)
        {
            SelectedNeighborhood = preselectedNeighborhoodId.HasValue
                ? Neighborhoods.FirstOrDefault(n => n.Id == preselectedNeighborhoodId.Value) ?? Neighborhoods[0]
                : Neighborhoods[0];
        }
    }

    public void LoadCityObjects()
    {
        if (_selectedNeighborhood == null) return;
        CityObjects = new ObservableCollection<CityObjectItemViewModel>(
            _cityObjectService.GetAllByNeighborhood(_selectedNeighborhood.Id)
                .Select(dto => new CityObjectItemViewModel(dto)));
    }
}
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;

public class ManagerNoticeBoardViewModel : BaseViewModel
{
    private readonly BuildingService _buildingService;
    private readonly long _managerId;

    private ObservableCollection<BuildingDto> _buildings = new();
    public ObservableCollection<BuildingDto> Buildings
    {
        get => _buildings;
        private set => SetProperty(ref _buildings, value);
    }

    public ManagerNoticeBoardViewModel(long managerId)
    {
        _managerId = managerId;
        _buildingService = Injector.CreateInstance<BuildingService>();
        LoadBuildings();
    }

    public void LoadBuildings()
    {
        List<BuildingDto> buildings = _buildingService.GetAllByManager(_managerId);
        Buildings = new ObservableCollection<BuildingDto>(buildings);
    }

    public BuildingDto? GetBuildingById(long id)
    {
        return _buildingService.GetById(id);
    }
}
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;
using CommunityHub.Application.Services.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Domain.Buildings.CommonRooms;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;

public class BuildingDetailsViewModel : BaseViewModel
{
    private readonly BuildingService _buildingService;
    private readonly CommonRoomService _commonRoomService;
    private readonly long _buildingId;

    private BuildingDto? _building;
    public BuildingDto? Building
    {
        get => _building;
        private set => SetProperty(ref _building, value);
    }

    private ObservableCollection<CommonRoomDto> _commonRooms = new();
    public ObservableCollection<CommonRoomDto> CommonRooms
    {
        get => _commonRooms;
        private set => SetProperty(ref _commonRooms, value);
    }

    private ObservableCollection<BuildingMembershipDto> _memberships = new();
    public ObservableCollection<BuildingMembershipDto> Memberships
    {
        get => _memberships;
        private set => SetProperty(ref _memberships, value);
    }

    public BuildingDetailsViewModel(long buildingId)
    {
        _buildingId = buildingId;
        _buildingService = Injector.CreateInstance<BuildingService>();
        _commonRoomService = Injector.CreateInstance<CommonRoomService>();
        LoadBuilding();
    }

    public void LoadBuilding()
    {
        Building = _buildingService.GetById(_buildingId);
        if (Building == null) return;
        Memberships = new ObservableCollection<BuildingMembershipDto>(Building.Memberships);
    }

    public void LoadCommonRooms()
    {
        List<CommonRoomDto> rooms = _commonRoomService.GetByBuilding(_buildingId);
        CommonRooms = new ObservableCollection<CommonRoomDto>(rooms);
    }

    public void CreateCommonRoom(string name, string description,
                                  int floorNumber, RentalType rentalType)
    {
        if (Building == null) return;
        _commonRoomService.Create(name, description, floorNumber, rentalType, _buildingId);
        LoadCommonRooms();
    }
}
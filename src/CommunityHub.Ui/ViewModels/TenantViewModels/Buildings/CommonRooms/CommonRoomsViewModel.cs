using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Services.Entities.Buildings.CommonRooms;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;

public class CommonRoomsViewModel : BaseViewModel
{
    private readonly CommonRoomService _commonRoomService;
    private readonly CommonRoomRequestService _requestService;
    private readonly long _tenantId;
    private readonly long _buildingId;

    private ObservableCollection<CommonRoomCardViewModel> _rooms = new();
    private string _roomCountText = string.Empty;

    public CommonRoomsViewModel(
        CommonRoomService commonRoomService,
        CommonRoomRequestService requestService,
        long tenantId,
        long buildingId)
    {
        _commonRoomService = commonRoomService;
        _requestService = requestService;
        _tenantId = tenantId;
        _buildingId = buildingId;
        LoadRooms();
    }

    public ObservableCollection<CommonRoomCardViewModel> Rooms
    {
        get => _rooms;
        private set => SetProperty(ref _rooms, value);
    }

    public string RoomCountText
    {
        get => _roomCountText;
        private set => SetProperty(ref _roomCountText, value);
    }

    public void LoadRooms()
    {
        List<CommonRoomDto> rooms = _commonRoomService.GetByBuilding(_buildingId);
        List<CommonRoomRequestDto> tenantRequests = _requestService.GetByTenantAndBuilding(_tenantId, _buildingId);

        List<CommonRoomCardViewModel> cards = rooms
            .Select(r => new CommonRoomCardViewModel(r, tenantRequests))
            .ToList();

        Rooms = new ObservableCollection<CommonRoomCardViewModel>(cards);
        RoomCountText = $"Showing {rooms.Count} common area{(rooms.Count == 1 ? "" : "s")}";
    }

    public void SendRequest(long commonRoomId, DateTime dateFrom, DateTime dateTo)
    {
        _requestService.CreateRequest(commonRoomId, _tenantId, dateFrom, dateTo);
        LoadRooms();
    }
}
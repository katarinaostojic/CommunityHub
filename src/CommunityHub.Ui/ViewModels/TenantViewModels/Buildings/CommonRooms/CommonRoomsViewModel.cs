using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Services.Buildings.CommonRooms;
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
    private string _dateError = string.Empty;
    private bool _hasDateError;

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

    public string DateError
    {
        get => _dateError;
        private set => SetProperty(ref _dateError, value);
    }

    public bool HasDateError
    {
        get => _hasDateError;
        private set => SetProperty(ref _hasDateError, value);
    }

    public void LoadRooms()
    {
        List<CommonRoomDto> rooms = _commonRoomService.GetByBuilding(_buildingId);
        List<CommonRoomRequestDto> tenantRequests = _requestService.GetByTenant(_tenantId);

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

    public bool TrySendRequest(long commonRoomId, DateTime? dateFrom, DateTime? dateTo)
    {
        HasDateError = dateFrom == null || dateTo == null;
        DateError = HasDateError ? "Please select both start and end date." : string.Empty;

        if (!HasDateError && dateTo!.Value.Date < dateFrom!.Value.Date)
        {
            HasDateError = true;
            DateError = "End date must be after start date.";
        }

        if (HasDateError) return false;

        SendRequest(commonRoomId, dateFrom!.Value, dateTo!.Value);
        HasDateError = false;
        DateError = string.Empty;
        return true;
    }
}
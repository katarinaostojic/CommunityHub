using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;

public class CommonRoomCardViewModel : BaseViewModel
{
    private readonly CommonRoomDto _room;

    public CommonRoomCardViewModel(CommonRoomDto room, List<CommonRoomRequestDto> tenantRequests)
    {
        _room = room;
        MyRequestsCount = tenantRequests.Count(r => r.CommonRoomId == room.Id);
    }

    public long Id => _room.Id;
    public string Name => _room.Name;
    public string Description => _room.Description;
    public string FloorDisplay => $"Floor {_room.FloorNumber}";
    public string RentalTypeDisplay => _room.RentalType == RentalType.PerDay ? "1 day only" : "Multiple days";
    public int MyRequestsCount { get; }
    public string MyRequestsLabel => $"My requests ({MyRequestsCount})";
}
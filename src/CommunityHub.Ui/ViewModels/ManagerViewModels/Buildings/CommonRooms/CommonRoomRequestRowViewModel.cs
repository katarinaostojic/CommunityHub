using CommunityHub.Application.Domain.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.CommonRooms;

public class CommonRoomRequestRowViewModel
{
    private readonly CommonRoomRequestDto _dto;

    public CommonRoomRequestRowViewModel(CommonRoomRequestDto dto)
    {
        _dto = dto;
    }

    public long Id => _dto.Id;
    public string TenantFullName => _dto.TenantFullName;
    public DateTime? ApprovedDate => _dto.ApprovedDate;
    public DateTime? ProposedDateFrom => _dto.ProposedDateFrom;
    public DateTime? ProposedDateTo => _dto.ProposedDateTo;

    public string StatusDisplay => _dto.Status switch
    {
        CommonRoomRequestStatus.Pending => "Pending",
        CommonRoomRequestStatus.Approved => "Approved",
        CommonRoomRequestStatus.Rejected => "Rejected",
        CommonRoomRequestStatus.PendingDateChange => "Pending date change",
        _ => _dto.Status.ToString()
    };

    public string StatusBadgeColor => _dto.Status switch
    {
        CommonRoomRequestStatus.Pending => "#F39C12",
        CommonRoomRequestStatus.Approved => "#27AE60",
        CommonRoomRequestStatus.Rejected => "#E74C3C",
        CommonRoomRequestStatus.PendingDateChange => "#8E44AD",
        _ => "#95A5A6"
    };

    public bool ShowPerDayActions =>
        _dto.Status == CommonRoomRequestStatus.Pending &&
        _dto.RentalType == RentalType.PerDay;

    public bool ShowMultiDayActions =>
        _dto.Status == CommonRoomRequestStatus.Pending &&
        _dto.RentalType == RentalType.MultiDay;
}
using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;

public class CommonRoomRequestRowViewModel : BaseViewModel
{
    private readonly CommonRoomRequestDto _request;

    public CommonRoomRequestRowViewModel(CommonRoomRequestDto request)
    {
        _request = request;
    }

    public long Id => _request.Id;
    public string CommonRoomName => _request.CommonRoomName;

    public string DateRange =>
        $"{_request.DateFrom:dd.MM.yyyy} - {_request.DateTo:dd.MM.yyyy}";

    public string RentalTypeDisplay => _request.RentalType == RentalType.PerDay
        ? "Per day"
        : "Multi day";

    public CommonRoomRequestStatus Status => _request.Status;

    public string StatusDisplay => _request.Status switch
    {
        CommonRoomRequestStatus.Pending => "⏳ Pending Approval",
        CommonRoomRequestStatus.Approved => "✔ Approved",
        CommonRoomRequestStatus.Rejected => "✕ Rejected",
        CommonRoomRequestStatus.PendingDateChange => "ⓘ Pending date change",
        _ => _request.Status.ToString()
    };

    public string ApprovedDateDisplay => _request.ApprovedDate.HasValue
        ? $"Approved for: {_request.ApprovedDate.Value:dd.MM.yyyy}"
        : string.Empty;

    public string ProposedDateRangeDisplay => _request.ProposedDateFrom.HasValue && _request.ProposedDateTo.HasValue
        ? $"Proposed: {_request.ProposedDateFrom.Value:dd.MM.yyyy} - {_request.ProposedDateTo.Value:dd.MM.yyyy}"
        : string.Empty;

    public bool CanCancel => _request.Status == CommonRoomRequestStatus.Pending
                          || _request.Status == CommonRoomRequestStatus.PendingDateChange;

    public bool CanAcceptDateChange => _request.Status == CommonRoomRequestStatus.PendingDateChange;

    public bool ApprovedDateVisible => _request.ApprovedDate.HasValue;

    public bool ProposedDateVisible => _request.ProposedDateFrom.HasValue;
}
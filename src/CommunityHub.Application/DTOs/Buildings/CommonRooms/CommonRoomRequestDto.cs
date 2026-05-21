using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

namespace CommunityHub.Application.DTOs.Buildings.CommonRooms;

public class CommonRoomRequestDto
{
    public long Id { get; init; }
    public long CommonRoomId { get; init; }
    public string CommonRoomName { get; init; }
    public string TenantFullName { get; init; }
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public CommonRoomRequestStatus Status { get; init; }
    public DateTime? ApprovedDate { get; init; }
    public DateTime? ProposedDateFrom { get; init; }
    public DateTime? ProposedDateTo { get; init; }
    public RentalType RentalType { get; init; }

    public CommonRoomRequestDto(
        long id,
        long commonRoomId,
        string commonRoomName,
        string tenantFullName,
        DateTime dateFrom,
        DateTime dateTo,
        CommonRoomRequestStatus status,
        RentalType rentalType,
        DateTime? approvedDate = null,
        DateTime? proposedDateFrom = null,
        DateTime? proposedDateTo = null)
    {
        Id = id;
        CommonRoomId = commonRoomId;
        CommonRoomName = commonRoomName;
        TenantFullName = tenantFullName;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Status = status;
        RentalType = rentalType;
        ApprovedDate = approvedDate;
        ProposedDateFrom = proposedDateFrom;
        ProposedDateTo = proposedDateTo;
    }

    public bool CanBeCancelled
        => Status == CommonRoomRequestStatus.Pending ||
           Status == CommonRoomRequestStatus.PendingDateChange;

    public bool CanAcceptProposedDateChange
        => Status == CommonRoomRequestStatus.PendingDateChange &&
           ProposedDateFrom != null &&
           ProposedDateTo != null;
}
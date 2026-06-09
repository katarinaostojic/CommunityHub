using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

public enum CommonRoomRequestStatus
{
    Pending,
    Approved,
    Rejected,
    PendingDateChange
}

public class CommonRoomRequest
{
    public long Id { get; private set; }
    public CommonRoom CommonRoom { get; private set; }
    public User Tenant { get; private set; }
    public DateTime DateFrom { get; private set; }
    public DateTime DateTo { get; private set; }
    public CommonRoomRequestStatus Status { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public DateTime? ProposedDateFrom { get; private set; }
    public DateTime? ProposedDateTo { get; private set; }

    public CommonRoomRequest(long id, CommonRoom commonRoom, User tenant,
        DateTime dateFrom, DateTime dateTo, CommonRoomRequestStatus status,
        DateTime? approvedDate = null, DateTime? proposedDateFrom = null,
        DateTime? proposedDateTo = null)
    {
        Id = id;
        CommonRoom = commonRoom;
        Tenant = tenant;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Status = status;
        ApprovedDate = approvedDate;
        ProposedDateFrom = proposedDateFrom;
        ProposedDateTo = proposedDateTo;
    }

    public bool CanBeCancelled
        => CommonRoomRequestRules.CanBeCancelled(Status);

    public bool CanAcceptProposedDateChange
        => CommonRoomRequestRules.CanAcceptProposedDateChange(
            Status,
            ProposedDateFrom,
            ProposedDateTo);

    public int RequestedDays => (int)(DateTo - DateFrom).TotalDays + 1;

    public void EnsureCanBeCancelled()
    {
        CommonRoomRequestRules.EnsureCanBeCancelled(Status);
    }

    public bool CanBeAutoApproved(bool isRequestedRangeFree)
    {
        return CommonRoomRequestRules.CanBeAutoApproved(
            CommonRoom.IsMultiDayRental,
            isRequestedRangeFree);
    }

    public void AutoApprove()
    {
        Status = CommonRoomRequestStatus.Approved;
    }

    public void Reject()
    {
        Status = CommonRoomRequestStatus.Rejected;
    }

    public void ProposeNewDateRange(DateTime newDateFrom, DateTime newDateTo)
    {
        Status = CommonRoomRequestStatus.PendingDateChange;
        ProposedDateFrom = newDateFrom;
        ProposedDateTo = newDateTo;
    }

    public void ApproveWithDate(DateTime approvedDate)
    {
        Status = CommonRoomRequestStatus.Approved;
        ApprovedDate = approvedDate;
    }

    public void AcceptProposedDates()
    {
        CommonRoomRequestRules.EnsureCanAcceptProposedDateChange(
            Status,
            ProposedDateFrom,
            ProposedDateTo);

        DateFrom = ProposedDateFrom!.Value;
        DateTo = ProposedDateTo!.Value;
        ProposedDateFrom = null;
        ProposedDateTo = null;
        Status = CommonRoomRequestStatus.Pending;
    }

    public static string? ValidateDateRange(DateTime dateFrom, DateTime dateTo)
    {
        return CommonRoomRequestRules.ValidateDateRange(dateFrom, dateTo);
    }
}
using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Application.Domain.Buildings.CommonRooms;

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
        if (ProposedDateFrom == null || ProposedDateTo == null) return;
        DateFrom = ProposedDateFrom.Value;
        DateTo = ProposedDateTo.Value;
        ProposedDateFrom = null;
        ProposedDateTo = null;
        Status = CommonRoomRequestStatus.Pending;
    }
}
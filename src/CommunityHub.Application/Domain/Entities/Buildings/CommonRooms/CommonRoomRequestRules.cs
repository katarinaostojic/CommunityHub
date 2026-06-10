namespace CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

public static class CommonRoomRequestRules
{
    public static bool CanBeCancelled(CommonRoomRequestStatus status)
    {
        return status == CommonRoomRequestStatus.Pending
            || status == CommonRoomRequestStatus.PendingDateChange;
    }

    public static bool CanAcceptProposedDateChange(
        CommonRoomRequestStatus status,
        DateTime? proposedDateFrom,
        DateTime? proposedDateTo)
    {
        return status == CommonRoomRequestStatus.PendingDateChange
            && proposedDateFrom != null
            && proposedDateTo != null;
    }

    public static bool CanBeAutoApproved(bool isMultiDayRental, bool isRequestedRangeFree)
    {
        return isMultiDayRental && isRequestedRangeFree;
    }

    public static void EnsureCanBeCancelled(CommonRoomRequestStatus status)
    {
        if (!CanBeCancelled(status))
            throw new InvalidOperationException("Only pending requests can be cancelled.");
    }

    public static void EnsureCanAcceptProposedDateChange(
        CommonRoomRequestStatus status,
        DateTime? proposedDateFrom,
        DateTime? proposedDateTo)
    {
        if (!CanAcceptProposedDateChange(status, proposedDateFrom, proposedDateTo))
            throw new InvalidOperationException("Only requests with proposed date changes can be accepted.");
    }

    public static string? ValidateDateRange(DateTime dateFrom, DateTime dateTo)
    {
        if (dateFrom.Date < DateTime.Today)
            return "Start date cannot be in the past.";

        if (dateTo.Date < DateTime.Today)
            return "End date cannot be in the past.";

        if (dateTo.Date < dateFrom.Date)
            return "End date must be after start date.";

        return null;
    }
}
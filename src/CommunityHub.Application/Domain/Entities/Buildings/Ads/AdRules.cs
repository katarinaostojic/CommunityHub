namespace CommunityHub.Application.Domain.Entities.Buildings.Ads;

public static class AdRules
{
    public static bool IsActive(AdStatus status)
    {
        return status == AdStatus.Active;
    }

    public static bool IsExpired(AdStatus status, DateOnly dateTo, DateOnly today)
    {
        return IsActive(status) && dateTo < today;
    }

    public static bool OverlapsWith(
        DateOnly dateFrom,
        DateOnly dateTo,
        DateOnly otherFrom,
        DateOnly otherTo)
    {
        return dateFrom <= otherTo && dateTo >= otherFrom;
    }

    public static AdType GetOppositeType(AdType type)
    {
        return type == AdType.Offering
            ? AdType.Seeking
            : AdType.Offering;
    }

    public static bool IsEligibleMatchFor(Ad ad, Ad other)
    {
        return IsActive(ad.Status)
            && IsActive(other.Status)
            && ad.Author.Id != other.Author.Id
            && ad.Type == GetOppositeType(other.Type)
            && ad.Category == other.Category
            && OverlapsWith(ad.DateFrom, ad.DateTo, other.DateFrom, other.DateTo);
    }

    public static string? ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return "Description is required.";

        return null;
    }

    public static string? ValidateDateRange(DateOnly dateFrom, DateOnly dateTo)
    {
        if (dateFrom < DateOnly.FromDateTime(DateTime.Today))
            return "Dates cannot be in the past.";

        if (dateFrom > dateTo)
            return "Start date must be before end date.";

        return null;
    }
}
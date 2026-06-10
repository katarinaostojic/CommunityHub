using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Ui.Extensions;
using CommunityHub.Ui.Extensions.Buildings.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Notifications;

public static class NotificationItemTextBuilder
{
    public static string GetCategoryDisplay(AdNotificationDto notification)
    {
        return notification.Ad.Category.ToDisplayString();
    }

    public static string GetRelatedAdTypeDisplay(AdNotificationDto notification)
    {
        return notification.RelatedAd.Type
            .ToDisplayString()
            .Replace("↑ ", "")
            .Replace("↓ ", "");
    }

    public static string GetDateRangeDisplay(AdNotificationDto notification)
    {
        return FormatDateRange(notification.RelatedAd.DateFrom, notification.RelatedAd.DateTo);
    }

    public static string GetTimeDisplay(AdNotificationDto notification)
    {
        return FormatTime(notification.CreatedAt);
    }

    public static string GetIconGlyph(AdNotificationDto notification)
    {
        return notification.Type == AdNotificationType.Booking ? "🔔" : "✦";
    }

    public static string GetTitle(AdNotificationDto notification)
    {
        return notification.Type == AdNotificationType.Booking
            ? "New booking on your ad"
            : "New matching ad found";
    }

    public static string GetBody(
        AdNotificationDto notification,
        string categoryDisplay,
        string relatedAdTypeDisplay,
        string dateRangeDisplay)
    {
        if (notification.Type == AdNotificationType.Booking)
            return $"{notification.RelatedAd.AuthorName} booked slots for your \"{categoryDisplay}\" ad · {dateRangeDisplay}";

        return $"A new \"{categoryDisplay}\" ad appeared that matches your ad · {relatedAdTypeDisplay} · {dateRangeDisplay}";
    }

    private static string FormatDateRange(DateOnly dateFrom, DateOnly dateTo)
    {
        if (dateFrom == dateTo)
            return $"{dateFrom:dd.MM.}";

        return $"{dateFrom:dd.MM.} – {dateTo:dd.MM.}";
    }

    private static string FormatTime(DateTime createdAt)
    {
        DateTime today = DateTime.Today;

        if (createdAt.Date == today)
            return $"Today, {createdAt:HH:mm}";

        if (createdAt.Date == today.AddDays(-1))
            return $"Yesterday, {createdAt:HH:mm}";

        return $"{createdAt:dd.MM.yyyy}, {createdAt:HH:mm}";
    }
}
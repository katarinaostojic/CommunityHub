using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Ui.Extensions;
using CommunityHub.Ui.Extensions.Buildings.Ads;
using CommunityHub.Ui.ViewModels;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Notifications;

public class NotificationItemViewModel : BaseViewModel
{
    private bool _isRead;

    public NotificationItemViewModel(AdNotificationDto notification)
    {
        Id = notification.Id;
        Ad = notification.Ad;
        RelatedAd = notification.RelatedAd;
        Type = notification.Type;
        CreatedAt = notification.CreatedAt;
        _isRead = notification.IsRead;

        CategoryDisplay = notification.Ad.Category.ToDisplayString();
        RelatedAdTypeDisplay = notification.RelatedAd.Type.ToDisplayString().Replace("↑ ", "").Replace("↓ ", "");
        DateRangeDisplay = FormatDateRange(notification.RelatedAd.DateFrom, notification.RelatedAd.DateTo);
        TimeDisplay = FormatTime(notification.CreatedAt);

        IconGlyph = Type == AdNotificationType.Booking ? "🔔" : "✦";
        Title = Type == AdNotificationType.Booking
            ? "New booking on your ad"
            : "New matching ad found";

        Body = Type == AdNotificationType.Booking
            ? $"{notification.RelatedAd.AuthorName} booked slots for your \"{CategoryDisplay}\" ad · {DateRangeDisplay}"
            : $"A new \"{CategoryDisplay}\" ad appeared that matches your ad · {RelatedAdTypeDisplay} · {DateRangeDisplay}";

        ActionText = "View Slots →";
    }

    public long Id { get; }
    public AdDto Ad { get; }
    public AdDto RelatedAd { get; }
    public AdNotificationType Type { get; }
    public DateTime CreatedAt { get; }

    public string IconGlyph { get; }
    public string Title { get; }
    public string Body { get; }
    public string TimeDisplay { get; }
    public string ActionText { get; }
    public string CategoryDisplay { get; }
    public string RelatedAdTypeDisplay { get; }
    public string DateRangeDisplay { get; }

    public bool IsRead
    {
        get => _isRead;
        private set
        {
            if (!SetProperty(ref _isRead, value))
                return;

            OnPropertyChanged(nameof(IsUnread));
        }
    }

    public bool IsUnread => !IsRead;

    public void MarkAsRead()
    {
        IsRead = true;
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
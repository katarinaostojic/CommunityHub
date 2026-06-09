using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
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

        string categoryDisplay = NotificationItemTextBuilder.GetCategoryDisplay(notification);
        string relatedAdTypeDisplay = NotificationItemTextBuilder.GetRelatedAdTypeDisplay(notification);
        string dateRangeDisplay = NotificationItemTextBuilder.GetDateRangeDisplay(notification);

        TimeDisplay = NotificationItemTextBuilder.GetTimeDisplay(notification);
        IconGlyph = NotificationItemTextBuilder.GetIconGlyph(notification);
        Title = NotificationItemTextBuilder.GetTitle(notification);
        Body = NotificationItemTextBuilder.GetBody(
            notification,
            categoryDisplay,
            relatedAdTypeDisplay,
            dateRangeDisplay);

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
}
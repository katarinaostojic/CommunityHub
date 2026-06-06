using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Ui.Extensions;
using CommunityHub.Ui.Extensions.Buildings.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;

public class AdNotificationViewModel : BaseViewModel
{
    public AdNotificationViewModel(AdNotificationDto notification)
    {
        Id = notification.Id;
        AdId = notification.Ad.Id;
        RelatedAdId = notification.RelatedAd.Id;
        Type = notification.Type;

        RelatedTenantName = notification.RelatedAd.AuthorName;
        AdCategoryDisplay = notification.Ad.Category.ToDisplayString();
        RelatedAdDescription = notification.RelatedAd.Description;
        TimeDisplay = notification.CreatedAt.ToString("dd.MM. HH:mm");
    }

    public long Id { get; }
    public long AdId { get; }
    public long RelatedAdId { get; }
    public AdNotificationType Type { get; }

    public string RelatedTenantName { get; }
    public string AdCategoryDisplay { get; }
    public string RelatedAdDescription { get; }
    public string TimeDisplay { get; }

    public bool IsMatchingAdNotification => Type == AdNotificationType.MatchingAd;

    public string Title => IsMatchingAdNotification
        ? $"New matching ad for your \"{AdCategoryDisplay}\" ad"
        : $"New booking on your \"{AdCategoryDisplay}\" ad";

    public string Body => IsMatchingAdNotification
        ? $"{RelatedTenantName} posted: \"{RelatedAdDescription}\""
        : $"{RelatedTenantName}: \"{RelatedAdDescription}\"";

    public string ActionText => IsMatchingAdNotification ? "View Slots →" : "View Ad →";
}
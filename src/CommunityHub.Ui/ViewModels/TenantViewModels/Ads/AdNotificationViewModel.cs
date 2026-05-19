using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class AdNotificationViewModel : BaseViewModel
{
    public AdNotificationViewModel(AdNotificationDto notification)
    {
        Id = notification.Id;
        AdId = notification.Ad.Id;
        BookedByTenantName = notification.BookedByAd.AuthorName;
        AdCategoryDisplay = notification.Ad.Category.ToDisplayString();
        BookedByAdDescription = notification.BookedByAd.Description;
        TimeDisplay = notification.CreatedAt.ToString("dd.MM. HH:mm");
    }

    public long Id { get; }
    public long AdId { get; }
    public string BookedByTenantName { get; }
    public string AdCategoryDisplay { get; }
    public string BookedByAdDescription { get; }
    public string TimeDisplay { get; }

    public string Title => $"New booking on your \"{AdCategoryDisplay}\" ad";
    public string Body => $"{BookedByTenantName}: \"{BookedByAdDescription}\"";
}
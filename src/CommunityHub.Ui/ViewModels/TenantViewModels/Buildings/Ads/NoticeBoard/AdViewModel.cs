using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Ui.Extensions;
using CommunityHub.Ui.Extensions.Buildings.Ads;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;

public class AdViewModel : BaseViewModel
{
    private readonly AdDto _ad;
    private readonly long _currentUserId;

    public AdViewModel(AdDto ad, long currentUserId, long? myMatchingAdId = null)
    {
        _ad = ad;
        _currentUserId = currentUserId;
        MyMatchingAdId = myMatchingAdId;
    }

    public long Id => _ad.Id;
    public string AuthorName => _ad.AuthorName;
    public string Description => _ad.Description;
    public string TypeDisplay => _ad.Type.ToDisplayString();
    public string CategoryDisplay => _ad.Category.ToDisplayString();
    public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

    public bool IsOwnAd => _ad.AuthorId == _currentUserId;
    public long? MyMatchingAdId { get; }

    public Visibility ArchiveButtonVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ViewSlotsVisible => (!IsOwnAd && MyMatchingAdId != null) ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ViewDetailsVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;

    public string ViewDetailsDisplay => "→ View details";
}
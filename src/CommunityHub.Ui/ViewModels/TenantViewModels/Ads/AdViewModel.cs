using CommunityHub.Application.Domain.Ads;
using CommunityHub.Ui.Extensions;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class AdViewModel : BaseViewModel
{
    private readonly Ad _ad;
    private readonly long _currentUserId;

    public AdViewModel(Ad ad, long currentUserId, Ad? myMatchingAd = null)
    {
        _ad = ad;
        _currentUserId = currentUserId;
        MyMatchingAd = myMatchingAd;
    }

    public long Id => _ad.Id;
    public string AuthorName => _ad.Author.DisplayName;
    public string Description => _ad.Description;
    public string TypeDisplay => _ad.Type.ToDisplayString();
    public string CategoryDisplay => _ad.Category.ToDisplayString();
    public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

    public bool IsOwnAd => _ad.Author.Id == _currentUserId;
    public Ad? MyMatchingAd { get; }

    public Visibility ArchiveButtonVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ViewSlotsVisible => (!IsOwnAd && MyMatchingAd != null) ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ViewBookingsVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;

    public string ViewBookingsDisplay => _ad.Slots.Any(s => !s.IsFree)
        ? $"→ View bookings ({_ad.Slots.Count(s => !s.IsFree)})"
        : "→ View details";
}
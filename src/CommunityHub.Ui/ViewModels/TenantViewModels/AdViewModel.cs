using CommunityHub.Application.Domain.Ads;
using CommunityHub.Ui.Extensions;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

public class AdViewModel : BaseViewModel
{
    private readonly Ad _ad;
    private readonly long _currentUserId;

    public AdViewModel(Ad ad, long currentUserId)
    {
        _ad = ad;
        _currentUserId = currentUserId;
    }

    public long Id => _ad.Id;
    public string AuthorName => _ad.Author.DisplayName;
    public string Description => _ad.Description;
    public string TypeDisplay => _ad.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";
    public string CategoryDisplay => _ad.Category.ToDisplayString();
    public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

    public bool IsOwnAd => _ad.Author.Id == _currentUserId;

    public Visibility ArchiveButtonVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ViewSlotsVisible => !IsOwnAd ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ViewBookingsVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;

    public string ViewBookingsDisplay => _ad.Slots.Any(s => !s.IsFree)
        ? $"→ View bookings ({_ad.Slots.Count(s => !s.IsFree)})"
        : "→ View details";
}
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Extensions;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class AdDetailsViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _adId;

    public AdDetailsViewModel(Ad ad, AdService adService)
    {
        _adService = adService;
        _adId = ad.Id;

        TypeDisplay = ad.Type.ToDisplayString();
        CategoryDisplay = ad.Category.ToDisplayString();
        DateRangeDisplay = $"{ad.DateFrom:dd.MM.} - {ad.DateTo:dd.MM.yyyy}";
        Description = ad.Description;
        IsActive = ad.IsActive;

        Slots = new AdSlotsViewModel(adService, ad.Id, ad.DateFrom, ad.DateTo);
    }

    public string TypeDisplay { get; }
    public string CategoryDisplay { get; }
    public string DateRangeDisplay { get; }
    public string Description { get; }
    public bool IsActive { get; }
    public AdSlotsViewModel Slots { get; }

    public Visibility ArchivedWarningVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;
    public Visibility ArchiveButtonVisible => IsActive ? Visibility.Visible : Visibility.Collapsed;
    public Visibility RestoreButtonVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;
    public Visibility AdStatusTextVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;

    public void ArchiveAd() => _adService.Archive(_adId);

    public void RestoreAd() => _adService.Restore(_adId);

    public Ad? GetRefreshedAd() => _adService.GetById(_adId);
}
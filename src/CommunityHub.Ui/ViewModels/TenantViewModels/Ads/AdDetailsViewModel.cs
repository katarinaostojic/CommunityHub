using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Services.Interfaces.Ads;
using CommunityHub.Ui.Extensions;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads.Booking;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class AdDetailsViewModel : BaseViewModel
{
    private readonly IAdService _adService;
    private readonly long _adId;
    private bool _isActive;

    public AdDetailsViewModel(
        AdDto ad,
        IAdService adService,
        IAdSlotBookingService slotBookingService)
    {
        _adService = adService;
        _adId = ad.Id;

        TypeDisplay = ad.Type.ToDisplayString();
        CategoryDisplay = ad.Category.ToDisplayString();
        DateRangeDisplay = $"{ad.DateFrom:dd.MM.} - {ad.DateTo:dd.MM.yyyy}";
        Description = ad.Description;
        _isActive = ad.IsActive;

        Slots = new AdSlotsViewModel(slotBookingService, ad.Id, ad.DateFrom, ad.DateTo);
    }

    public string TypeDisplay { get; }
    public string CategoryDisplay { get; }
    public string DateRangeDisplay { get; }
    public string Description { get; }
    public AdSlotsViewModel Slots { get; }

    public bool IsActive
    {
        get => _isActive;
        private set
        {
            if (SetProperty(ref _isActive, value))
            {
                OnPropertyChanged(nameof(ArchivedWarningVisible));
                OnPropertyChanged(nameof(ArchiveButtonVisible));
                OnPropertyChanged(nameof(RestoreButtonVisible));
                OnPropertyChanged(nameof(AdStatusTextVisible));
            }
        }
    }

    public Visibility ArchivedWarningVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;
    public Visibility ArchiveButtonVisible => IsActive ? Visibility.Visible : Visibility.Collapsed;
    public Visibility RestoreButtonVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;
    public Visibility AdStatusTextVisible => IsActive ? Visibility.Collapsed : Visibility.Visible;

    public void ArchiveAd()
    {
        _adService.Archive(_adId);
        IsActive = false;
    }

    public void RestoreAd()
    {
        _adService.Restore(_adId);
        IsActive = true;
    }
}
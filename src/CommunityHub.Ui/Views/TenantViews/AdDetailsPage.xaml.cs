using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class AdDetailsPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly Ad _ad;
    private readonly AdService _adService;

    public AdDetailsPage(User user, BuildingMembership membership, Ad ad)
    {
        InitializeComponent();
        _adService = ServiceFactory.CreateAdService();
        _user = user;
        _membership = membership;
        _ad = ad;
        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
        DisplayAdSummary();
        LoadBookedSlots();
        LoadFreeSlots();
    }

    private void DisplayAdSummary()
    {
        AdTypeText.Text = _ad.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";
        AdCategoryText.Text = GetCategoryDisplay(_ad.Category);
        AdDateRangeText.Text = $"{_ad.DateFrom:dd.MM.} - {_ad.DateTo:dd.MM.yyyy}";
        AdDescriptionText.Text = _ad.Description;

        if (_ad.IsActive)
        {
            ArchivedWarning.Visibility = Visibility.Collapsed;
            ArchiveButton.Visibility = Visibility.Visible;
            RestoreButton.Visibility = Visibility.Collapsed;
            AdStatusText.Visibility = Visibility.Collapsed;
        }
        else
        {
            ArchivedWarning.Visibility = Visibility.Visible;
            ArchiveButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Visible;
            AdStatusText.Visibility = Visibility.Visible;
        }
    }

    private void LoadBookedSlots()
    {
        List<AdSlot> bookedSlots = _adService.GetBookedSlots(_ad.Id);
        BookedSlotsTitleText.Text = $"Booked slots ({bookedSlots.Count})";

        List<BookedSlotsByTenantGroup> grouped = bookedSlots
            .GroupBy(s => s.BookedByAdId)
            .Select(g =>
            {
                Ad? bookedByAd = g.Key.HasValue ? _adService.GetById(g.Key.Value) : null;
                return new BookedSlotsByTenantGroup(g.ToList(), bookedByAd);
            })
            .ToList();

        BookedSlotsPanel.ItemsSource = grouped;
    }

    private void LoadFreeSlots()
    {
        List<AdSlot> freeSlots = _adService.GetFreeSlots(_ad.Id, _ad.DateFrom, _ad.DateTo);

        List<FreeSlotDayGroup> grouped = freeSlots
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new FreeSlotDayGroup(g.Key, g.ToList()))
            .ToList();

        FreeSlotsPanel.ItemsSource = grouped;
    }

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        _adService.Archive(_ad.Id);
        NavigationService.Navigate(new AdDetailsPage(_user, _membership, _adService.GetById(_ad.Id)!));
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        _adService.Restore(_ad.Id);
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }

    private static string GetCategoryDisplay(AdCategory category) => category switch
    {
        AdCategory.Moving => "Moving",
        AdCategory.ApplianceRepair => "Appliance repair",
        AdCategory.Lending => "Lending",
        AdCategory.Cleaning => "Cleaning",
        AdCategory.Other => "Other",
        _ => category.ToString()
    };

    private class BookedSlotsByTenantGroup
    {
        private readonly Ad? _bookedByAd;

        public BookedSlotsByTenantGroup(List<AdSlot> slots, Ad? bookedByAd)
        {
            _bookedByAd = bookedByAd;
            Slots = slots.Select(s => new SlotChipDisplay(s)).ToList();
        }

        public string TenantName => _bookedByAd?.Author.DisplayName ?? "Unknown";
        public List<SlotChipDisplay> Slots { get; }
        public string TheirAdDescription => _bookedByAd?.Description ?? string.Empty;
        public string TheirAdType => _bookedByAd?.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";
        public string TheirAdCategory => _bookedByAd != null ? GetCategoryDisplay(_bookedByAd.Category) : string.Empty;
        public string TheirAdDateRange => _bookedByAd != null
            ? $"{_bookedByAd.DateFrom:dd.MM.} – {_bookedByAd.DateTo:dd.MM.yyyy}"
            : string.Empty;

        private static string GetCategoryDisplay(AdCategory category) => category switch
        {
            AdCategory.Moving => "Moving",
            AdCategory.ApplianceRepair => "Appliance repair",
            AdCategory.Lending => "Lending",
            AdCategory.Cleaning => "Cleaning",
            AdCategory.Other => "Other",
            _ => category.ToString()
        };
    }

    private class SlotChipDisplay
    {
        private readonly AdSlot _slot;

        public SlotChipDisplay(AdSlot slot)
        {
            _slot = slot;
        }

        public string Display => $"{_slot.StartTime:HH:mm} - {_slot.EndTime:HH:mm}";
    }

    private class FreeSlotDayGroup
    {
        public FreeSlotDayGroup(DateOnly date, List<AdSlot> slots)
        {
            DateDisplay = date.ToString("dd.MM");
            Slots = slots.Select(s => new SlotChipDisplay(s)).ToList();
        }

        public string DateDisplay { get; }
        public List<SlotChipDisplay> Slots { get; }
    }
}
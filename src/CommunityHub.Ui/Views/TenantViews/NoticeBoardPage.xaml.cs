using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NoticeBoardPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly AdService _adService;
    private AdType? _currentTypeFilter = null;
    private AdCategory? _currentCategoryFilter = null;
    private List<Ad> _allActiveAds = new List<Ad>();

    public NoticeBoardPage(User user, BuildingMembership membership)
    {
        InitializeComponent();
        _adService = ServiceFactory.CreateAdService();
        _user = user;
        _membership = membership;
        UserNameTextBlock.Text = _user.DisplayName;
        BuildingSubtitle.Text = $"Building: {_membership.Building.Street} {_membership.Building.StreetNumber}, {_membership.Building.Neighborhood}";
        AppMenu.Initialize(_user);
        InitializeCategoryFilter();
        LoadAds();
    }

    private void InitializeCategoryFilter()
    {
        CategoryComboBox.Items.Add("All Categories");
        foreach (AdCategory category in Enum.GetValues<AdCategory>())
            CategoryComboBox.Items.Add(AdDisplay.GetCategoryDisplay(category));
        CategoryComboBox.SelectedIndex = 0;
    }

    private void LoadAds()
    {
        _allActiveAds = _adService.GetActiveByBuilding(_membership.Building.Id);
        DisplayAds();
    }

    private void DisplayAds()
    {
        List<Ad> filtered = _allActiveAds
            .Where(ad => _currentTypeFilter == null || ad.Type == _currentTypeFilter)
            .Where(ad => _currentCategoryFilter == null || ad.Category == _currentCategoryFilter)
            .ToList();

        AdsPanel.ItemsSource = filtered
            .Select(ad => new AdDisplay(ad, _user.Id))
            .ToList();

        ResultsCountText.Text = $"Showing {filtered.Count} active ad{(filtered.Count != 1 ? "s" : "")}";
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTypeFilter = null;
        DisplayAds();
    }

    private void FilterOfferingButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTypeFilter = AdType.Offering;
        DisplayAds();
    }

    private void FilterSeekingButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTypeFilter = AdType.Seeking;
        DisplayAds();
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedIndex == 0)
            _currentCategoryFilter = null;
        else
            _currentCategoryFilter = (AdCategory)(CategoryComboBox.SelectedIndex - 1);

        DisplayAds();
    }

    private void NewAdButton_Click(object sender, RoutedEventArgs e)
    {
    //    NavigationService.Navigate(new NewAdPage(_user, _membership));
    }

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        AdDisplay display = (AdDisplay)((Button)sender).Tag;
        _adService.Archive(display.Id);
        LoadAds();
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Request archived successfully.");
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
    //    AdDisplay display = (AdDisplay)((Button)sender).Tag;
    //    Ad? ad = _adService.GetById(display.Id);
    //    if (ad == null) return;
    //    NavigationService.Navigate(new ViewSlotsPage(_user, _membership, ad));
    }

    private void ViewBookingsButton_Click(object sender, RoutedEventArgs e)
    {
    //    AdDisplay display = (AdDisplay)((Button)sender).Tag;
    //    Ad? ad = _adService.GetById(display.Id);
    //    if (ad == null) return;
    //    NavigationService.Navigate(new AdDetailsPage(_user, _membership, ad));
    }

    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private class AdDisplay
    {
        private readonly Ad _ad;
        private readonly long _currentUserId;

        public AdDisplay(Ad ad, long currentUserId)
        {
            _ad = ad;
            _currentUserId = currentUserId;
        }

        public long Id => _ad.Id;
        public string AuthorName => _ad.Author.DisplayName;
        public string Description => _ad.Description;

        public string TypeDisplay => _ad.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";

        public string CategoryDisplay => GetCategoryDisplay(_ad.Category);

        public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

        public bool IsOwnAd => _ad.Author.Id == _currentUserId;

        public Visibility IsOwnAdVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ArchiveButtonVisible => IsOwnAd ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ViewSlotsVisible => !IsOwnAd ? Visibility.Visible : Visibility.Collapsed;

        public string ViewBookingsDisplay => $"→ View bookings ({_ad.Slots.Count(s => !s.IsFree)})";

        public Visibility ViewBookingsVisible
            => IsOwnAd && _ad.Slots.Any(s => !s.IsFree) ? Visibility.Visible : Visibility.Collapsed;

        public static string GetCategoryDisplay(AdCategory category) => category switch
        {
            AdCategory.Moving => "Moving",
            AdCategory.ApplianceRepair => "Appliance repair",
            AdCategory.Lending => "Lending",
            AdCategory.Cleaning => "Cleaning",
            AdCategory.Other => "Other",
            _ => category.ToString()
        };
    }
}
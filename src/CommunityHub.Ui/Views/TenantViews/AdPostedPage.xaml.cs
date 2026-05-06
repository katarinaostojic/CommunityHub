using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class AdPostedPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly Ad _postedAd;
    private readonly AdService _adService;

    public AdPostedPage(User user, BuildingMembership membership, Ad postedAd, List<Ad> matchingAds)
    {
        InitializeComponent();
        _adService = ServiceFactory.CreateAdService();
        _user = user;
        _membership = membership;
        _postedAd = postedAd;
        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
        DisplayPostedAdSummary();
        DisplayMatchingAds(matchingAds);
    }

    private void DisplayPostedAdSummary()
    {
        AdTypeText.Text = _postedAd.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";
        AdCategoryText.Text = GetCategoryDisplay(_postedAd.Category);
        AdDateRangeText.Text = $"{_postedAd.DateFrom:dd.MM.yyyy} – {_postedAd.DateTo:dd.MM.yyyy}";
        AdBuildingText.Text = $"{_membership.Building.Street} {_membership.Building.StreetNumber}";
    }

    private void DisplayMatchingAds(List<Ad> matchingAds)
    {
        if (matchingAds.Count == 0)
        {
            MatchingAdsTitle.Text = "No matching ads found at the moment. You will be notified when one appears.";
            return;
        }

        string oppositeType = _postedAd.Type == AdType.Offering ? "seeking" : "offering";
        MatchingAdsTitle.Text = $"{matchingAds.Count} tenant{(matchingAds.Count != 1 ? "s" : "")} {oppositeType} help in {GetCategoryDisplay(_postedAd.Category)} · overlapping dates";

        MatchingAdsPanel.ItemsSource = matchingAds
            .Select(ad => new MatchingAdDisplay(ad, _postedAd))
            .ToList();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new NoticeBoardPage(_user, _membership));
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        MatchingAdDisplay display = (MatchingAdDisplay)((Button)sender).Tag;
        Ad? ad = _adService.GetById(display.Id);
        if (ad == null) return;
        //NavigationService.Navigate(new ViewSlotsPage(_user, _membership, ad, _postedAd));
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

    private class MatchingAdDisplay
    {
        private readonly Ad _ad;
        private readonly Ad _myAd;

        public MatchingAdDisplay(Ad ad, Ad myAd)
        {
            _ad = ad;
            _myAd = myAd;
        }

        public long Id => _ad.Id;
        public string AuthorName => _ad.Author.DisplayName;
        public string Description => _ad.Description;
        public string TypeDisplay => _ad.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";

        public string CategoryDisplay => GetCategoryDisplay(_ad.Category);

        public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

        public string OverlapDisplay
        {
            get
            {
                DateOnly overlapFrom = _ad.DateFrom > _myAd.DateFrom ? _ad.DateFrom : _myAd.DateFrom;
                DateOnly overlapTo = _ad.DateTo < _myAd.DateTo ? _ad.DateTo : _myAd.DateTo;
                return $"Overlap: {overlapFrom:dd.MM.} – {overlapTo:dd.MM.}";
            }
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
    }
}
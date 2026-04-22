using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NewAdPage : Page
{
    private readonly User _user;
    private readonly BuildingMembership _membership;
    private readonly AdService _adService;
    private AdType _selectedType = AdType.Offering;

    public NewAdPage(User user, BuildingMembership membership)
    {
        InitializeComponent();
        _adService = ServiceFactory.CreateAdService();
        _user = user;
        _membership = membership;
        UserNameTextBlock.Text = _user.DisplayName;
        DateFromPicker.DisplayDateStart = DateTime.Today;
        DateToPicker.DisplayDateStart = DateTime.Today;
        BuildingSubtitle.Text = $"Building: {_membership.Building.Street} {_membership.Building.StreetNumber}, {_membership.Building.Neighborhood}";
        AppMenu.Initialize(_user);
        InitializeCategoryComboBox();
        UpdateTypeButtons();
    }

    private void InitializeCategoryComboBox()
    {
        foreach (AdCategory category in Enum.GetValues<AdCategory>())
            CategoryComboBox.Items.Add(GetCategoryDisplay(category));
        CategoryComboBox.SelectedIndex = 0;
    }

    private void UpdateTypeButtons()
    {
        OfferingButton.Style = _selectedType == AdType.Offering
            ? (Style)FindResource("FilterChipButtonActive")
            : (Style)FindResource("FilterChipButton");

        SeekingButton.Style = _selectedType == AdType.Seeking
            ? (Style)FindResource("FilterChipButtonActive")
            : (Style)FindResource("FilterChipButton");
    }

    private void OfferingButton_Click(object sender, RoutedEventArgs e)
    {
        _selectedType = AdType.Offering;
        UpdateTypeButtons();
    }

    private void SeekingButton_Click(object sender, RoutedEventArgs e)
    {
        _selectedType = AdType.Seeking;
        UpdateTypeButtons();
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void PostAdButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm()) return;

        DateOnly dateFrom = DateOnly.FromDateTime(DateFromPicker.SelectedDate!.Value);
        DateOnly dateTo = DateOnly.FromDateTime(DateToPicker.SelectedDate!.Value);
        AdCategory category = (AdCategory)CategoryComboBox.SelectedIndex;
        string description = DescriptionTextBox.Text.Trim();

        Ad newAd = _adService.CreateAd(
            _membership.Building.Id, _user.Id, _selectedType,
            category, description, dateFrom, dateTo);

        List<Ad> activeAds = _adService.GetActiveByBuilding(_membership.Building.Id);
        List<Ad> matchingAds = _adService.FindMatchingAds(newAd, activeAds);

        //NavigationService.Navigate(new AdPostedPage(_user, _membership, newAd, matchingAds));
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            ShowValidationError("Please enter a description.");
            return false;
        }

        if (DateFromPicker.SelectedDate == null || DateToPicker.SelectedDate == null)
        {
            ShowValidationError("Please select a date range.");
            return false;
        }

        if (DateFromPicker.SelectedDate.Value > DateToPicker.SelectedDate.Value)
        {
            ShowValidationError("Start date must be before end date.");
            return false;
        }

        ValidationErrorText.Visibility = Visibility.Collapsed;
        return true;
    }

    private void ShowValidationError(string message)
    {
        ValidationErrorText.Text = message;
        ValidationErrorText.Visibility = Visibility.Visible;
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
}
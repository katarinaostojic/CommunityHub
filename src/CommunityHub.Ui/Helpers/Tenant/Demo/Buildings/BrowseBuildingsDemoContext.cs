using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.Demo;

public class BrowseBuildingsDemoContext
{
    public TextBox SearchTextBox { get; }
    public TextBox StreetTextBox { get; }
    public TextBox NeighborhoodTextBox { get; }
    public TextBox CityTextBox { get; }
    public TextBox CountryTextBox { get; }
    public Border SuccessBanner { get; }
    public TextBlock SuccessTextBlock { get; }
    public Button ViewRequestsButton { get; }
    public FilterPanelAnimationHelper FilterPanel { get; }
    public BrowseBuildingsViewModel ViewModel { get; }
    public User User { get; }
    public Func<Window?> GetOwner { get; }

    public BrowseBuildingsDemoContext(
        TextBox searchTextBox,
        TextBox streetTextBox,
        TextBox neighborhoodTextBox,
        TextBox cityTextBox,
        TextBox countryTextBox,
        Border successBanner,
        TextBlock successTextBlock,
        Button viewRequestsButton,
        FilterPanelAnimationHelper filterPanel,
        BrowseBuildingsViewModel viewModel,
        User user,
        Func<Window?> getOwner)
    {
        SearchTextBox = searchTextBox;
        StreetTextBox = streetTextBox;
        NeighborhoodTextBox = neighborhoodTextBox;
        CityTextBox = cityTextBox;
        CountryTextBox = countryTextBox;
        SuccessBanner = successBanner;
        SuccessTextBlock = successTextBlock;
        ViewRequestsButton = viewRequestsButton;
        FilterPanel = filterPanel;
        ViewModel = viewModel;
        User = user;
        GetOwner = getOwner;
    }

    public void ResetFields()
    {
        SearchTextBox.Text = string.Empty;
        StreetTextBox.Text = string.Empty;
        NeighborhoodTextBox.Text = string.Empty;
        CityTextBox.Text = string.Empty;
        CountryTextBox.Text = string.Empty;
    }

    public void SearchAllBuildings()
    {
        ViewModel.Search(null, null, null, null);
    }

    public void SearchByMainText()
    {
        ViewModel.Search(OptionalText(SearchTextBox), null, null, null);
    }

    public void SearchByFilterFields()
    {
        ViewModel.Search(
            OptionalText(StreetTextBox),
            OptionalText(NeighborhoodTextBox),
            OptionalText(CityTextBox),
            OptionalText(CountryTextBox));
    }

    public BrowseBuildingsDemoState CaptureState()
    {
        return new BrowseBuildingsDemoState(
            SearchTextBox.Text,
            StreetTextBox.Text,
            NeighborhoodTextBox.Text,
            CityTextBox.Text,
            CountryTextBox.Text,
            SuccessBanner.Visibility,
            SuccessTextBlock.Text,
            ViewRequestsButton.Visibility);
    }

    public void RestoreState(BrowseBuildingsDemoState state)
    {
        SearchTextBox.Text = state.SearchText;
        StreetTextBox.Text = state.StreetText;
        NeighborhoodTextBox.Text = state.NeighborhoodText;
        CityTextBox.Text = state.CityText;
        CountryTextBox.Text = state.CountryText;
        SuccessBanner.Visibility = state.SuccessBannerVisibility;
        SuccessTextBlock.Text = state.SuccessText;
        ViewRequestsButton.Visibility = state.ViewRequestsButtonVisibility;
    }

    public void RestoreSearchResults()
    {
        if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
        {
            SearchByMainText();
            return;
        }

        SearchByFilterFields();
    }

    public void ShowRequestSentBanner(BuildingDto building)
    {
        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            $"✔ Demo: request for {building.FullAddress} was prepared successfully.");

        ViewRequestsButton.Visibility = Visibility.Visible;
    }

    public static string? OptionalText(TextBox textBox)
    {
        string text = textBox.Text.Trim();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
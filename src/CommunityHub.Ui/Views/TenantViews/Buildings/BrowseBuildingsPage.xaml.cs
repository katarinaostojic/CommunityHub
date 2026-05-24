using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Interfaces.Buildings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BrowseBuildingsPage : Page
{
    private readonly User _user;
    private readonly BrowseBuildingsViewModel _viewModel;
    private readonly FilterPanelAnimationHelper _filterPanel;

    public BrowseBuildingsPage(User user)
    {
        InitializeComponent();

        _user = user;

        IBuildingService buildingService = Injector.CreateInstance<IBuildingService>();
        _viewModel = new BrowseBuildingsViewModel(buildingService);

        _filterPanel = new FilterPanelAnimationHelper(Overlay, FilterPanelTranslate);

        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string? search = GetOptionalText(SearchTextBox);
        _viewModel.Search(search, null, null, null);
    }

    private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Search(
            GetOptionalText(FilterStreetTextBox),
            GetOptionalText(FilterNeighborhoodTextBox),
            GetOptionalText(FilterCityTextBox),
            GetOptionalText(FilterCountryTextBox));

        _filterPanel.Close();
    }

    private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingDto building = (BuildingDto)((Button)sender).Tag;

        BuildingAccessRequestDialog dialog = new BuildingAccessRequestDialog(building, _user)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            $"✔ Request Sent Successfully! The administrator of {building.FullAddress} has been notified.");

        ViewRequestsButton.Visibility = Visibility.Visible;
    }

    private void BuildingCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Button)
            return;

        BuildingDto building = (BuildingDto)((Border)sender).Tag;
        BuildingDto? fullBuilding = _viewModel.GetFullBuildingDto(building.Id);

        if (fullBuilding != null)
            NavigationService.Navigate(new BuildingDetailsPage(fullBuilding, _user));
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new MyBuildingRequestsPage(_user));

    private void PageButton_Click(object sender, RoutedEventArgs e)
    {
        string direction = (string)((Button)sender).Tag;

        if (direction == "Previous")
            _viewModel.PreviousPage();
        else
            _viewModel.NextPage();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) =>
        AppMenu.Open();

    private void FilterButton_Click(object sender, RoutedEventArgs e) =>
        _filterPanel.Toggle();

    private void Overlay_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) =>
        _filterPanel.Close();

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Text = string.Empty;
        FilterStreetTextBox.Text = string.Empty;
        FilterNeighborhoodTextBox.Text = string.Empty;
        FilterCityTextBox.Text = string.Empty;
        FilterCountryTextBox.Text = string.Empty;

        _viewModel.Search(null, null, null, null);
    }

    private static string? GetOptionalText(TextBox textBox)
    {
        string text = textBox.Text.Trim();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
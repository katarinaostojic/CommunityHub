using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Mappings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BrowseBuildingsPage : Page
{
    private readonly User _user;
    private readonly BrowseBuildingsViewModel _viewModel;
    private readonly BuildingService _buildingService;
    private bool _filterPanelOpen = false;

    public BrowseBuildingsPage(User user)
    {
        InitializeComponent();
        _user = user;

        _buildingService = Injector.CreateInstance<BuildingService>();
        _viewModel = new BrowseBuildingsViewModel(_buildingService);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = SearchTextBox.Text.Trim();
        _viewModel.Search(
            string.IsNullOrEmpty(search) ? null : search,
            null, null, null);
    }

    private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        string? street = string.IsNullOrWhiteSpace(FilterStreetTextBox.Text) ? null : FilterStreetTextBox.Text.Trim();
        string? neighborhood = string.IsNullOrWhiteSpace(FilterNeighborhoodTextBox.Text) ? null : FilterNeighborhoodTextBox.Text.Trim();
        string? city = string.IsNullOrWhiteSpace(FilterCityTextBox.Text) ? null : FilterCityTextBox.Text.Trim();
        string? country = string.IsNullOrWhiteSpace(FilterCountryTextBox.Text) ? null : FilterCountryTextBox.Text.Trim();

        _viewModel.Search(street, neighborhood, city, country);
        CloseFilterPanel();
    }

    private void ResetAll()
    {
        SearchTextBox.Text = string.Empty;
        FilterStreetTextBox.Text = string.Empty;
        FilterNeighborhoodTextBox.Text = string.Empty;
        FilterCityTextBox.Text = string.Empty;
        FilterCountryTextBox.Text = string.Empty;
        _viewModel.Search(null, null, null, null);
    }

    private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingDto buildingDto = (BuildingDto)((Button)sender).Tag;
        Building? building = _buildingService.GetById(buildingDto.Id);
        if (building == null) return;

        if (!ShowBuildingRequestAccessDialog(building)) return;

        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock,
            $"✔ Request Sent Successfully! The administrator of {buildingDto.FullAddress} has been notified.");
        ViewRequestsButton.Visibility = Visibility.Visible;
    }

    private bool ShowBuildingRequestAccessDialog(Building building)
    {
        BuildingAccessRequestDialog dialog = new BuildingAccessRequestDialog(building, _user);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void BuildingCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Button) return;
        BuildingDto buildingDto = (BuildingDto)((Border)sender).Tag;
        BuildingDto? fullDto = _viewModel.GetFullBuildingDto(buildingDto.Id);
        if (fullDto == null) return;
        NavigationService.Navigate(new BuildingDetailsPage(fullDto, _user));
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new MyBuildingRequestsPage(_user));

    private void PrevPageButton_Click(object sender, RoutedEventArgs e) => _viewModel.PreviousPage();

    private void NextPageButton_Click(object sender, RoutedEventArgs e) => _viewModel.NextPage();

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        if (_filterPanelOpen) CloseFilterPanel();
        else OpenFilterPanel();
    }

    private void OpenFilterPanel()
    {
        Overlay.Visibility = Visibility.Visible;
        DoubleAnimation animation = new DoubleAnimation
        {
            From = -300,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        FilterPanelTranslate.BeginAnimation(TranslateTransform.XProperty, animation);
        _filterPanelOpen = true;
    }

    private void CloseFilterPanel()
    {
        DoubleAnimation animation = new DoubleAnimation
        {
            From = 0,
            To = -300,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        animation.Completed += (s, e) => Overlay.Visibility = Visibility.Collapsed;
        FilterPanelTranslate.BeginAnimation(TranslateTransform.XProperty, animation);
        _filterPanelOpen = false;
    }

    private void Overlay_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) =>
        CloseFilterPanel();

    private void ResetFiltersButton_Click(object sender, RoutedEventArgs e) => ResetAll();

    private void ResetButton_Click(object sender, RoutedEventArgs e) => ResetAll();
}
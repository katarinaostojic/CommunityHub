using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
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
        _viewModel = CreateViewModel();
        _filterPanel = new FilterPanelAnimationHelper(Overlay, FilterPanelTranslate);

        DataContext = _viewModel;

        InitializeHeader();
    }

    private BrowseBuildingsViewModel CreateViewModel()
    {
        BuildingService buildingService = Injector.CreateInstance<BuildingService>();
        return new BrowseBuildingsViewModel(buildingService);
    }

    private void InitializeHeader()
    {
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
        BuildingDto building = GetBuildingFromButton(sender);

        if (!ShowBuildingRequestAccessDialog(building)) return;

        ShowRequestSentMessage(building);
    }

    private bool ShowBuildingRequestAccessDialog(BuildingDto building)
    {
        BuildingAccessRequestDialog dialog = new BuildingAccessRequestDialog(building, _user)
        {
            Owner = Window.GetWindow(this)
        };

        return dialog.ShowDialog() == true;
    }

    private void ShowRequestSentMessage(BuildingDto building)
    {
        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            $"✔ Request Sent Successfully! The administrator of {building.FullAddress} has been notified.");

        ViewRequestsButton.Visibility = Visibility.Visible;
    }

    private void BuildingCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Button) return;

        BuildingDto building = (BuildingDto)((Border)sender).Tag;
        BuildingDto? fullBuilding = _viewModel.GetFullBuildingDto(building.Id);

        if (fullBuilding != null)
            NavigationService.Navigate(new BuildingDetailsPage(fullBuilding, _user));
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new MyBuildingRequestsPage(_user));

    private void PrevPageButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.PreviousPage();

    private void NextPageButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.NextPage();

    private void MenuButton_Click(object sender, RoutedEventArgs e) =>
        AppMenu.Open();

    private void FilterButton_Click(object sender, RoutedEventArgs e) =>
        _filterPanel.Toggle();

    private void Overlay_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) =>
        _filterPanel.Close();

    private void ResetFiltersButton_Click(object sender, RoutedEventArgs e) =>
        ResetAll();

    private void ResetButton_Click(object sender, RoutedEventArgs e) =>
        ResetAll();

    private void ResetAll()
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

    private static BuildingDto GetBuildingFromButton(object sender)
    {
        return (BuildingDto)((Button)sender).Tag;
    }
}
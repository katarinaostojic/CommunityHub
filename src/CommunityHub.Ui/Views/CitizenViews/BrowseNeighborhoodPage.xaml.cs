using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class BrowseNeighborhoodPage : Window
{
    private readonly User _user;
    private readonly BrowseNeighborhoodViewModel _viewModel;
    private bool _filterPanelOpen = false;

    public BrowseNeighborhoodPage(User user, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;

        NeighborhoodService neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        _viewModel = new BrowseNeighborhoodViewModel(neighborhoodService);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Browse_Title") as string ?? "Neighborhoods");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Browse_Title") as string ?? "Neighborhoods");
        NavBar.SetUrl("communityhub://neighborhoods");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        NavBar.ReloadRequested += () => _viewModel.Reset();
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        => _viewModel.Search(SearchTextBox.Text);

    private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        string? name = NullIfBlank(FilterNameTextBox.Text);
        string? address = NullIfBlank(FilterAddressTextBox.Text);
        string? city = NullIfBlank(FilterCityTextBox.Text);
        string? country = NullIfBlank(FilterCountryTextBox.Text);
        _viewModel.ApplyFilters(name, address, city, country);
        CloseFilterPanel();
    }

    private static string? NullIfBlank(string value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void ResetAll()
    {
        SearchTextBox.Text = string.Empty;
        FilterNameTextBox.Text = string.Empty;
        FilterAddressTextBox.Text = string.Empty;
        FilterCityTextBox.Text = string.Empty;
        FilterCountryTextBox.Text = string.Empty;
        _viewModel.Reset();
    }

    private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodDto neighborhood = (NeighborhoodDto)((Button)sender).Tag;
        NeighborhoodAccessRequestDialog dialog = new NeighborhoodAccessRequestDialog(_user, neighborhood);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
    }

    private void MyRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        new MyRequestsPage(_user).Show();
        Close();
    }


    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        if (_filterPanelOpen) CloseFilterPanel();
        else OpenFilterPanel();
    }

    private void OpenFilterPanel()
    {
        Overlay.Visibility = Visibility.Visible;
        var animation = new DoubleAnimation { From = -320, To = 0, Duration = TimeSpan.FromMilliseconds(250) };
        FilterPanelTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animation);
        _filterPanelOpen = true;
    }

    private void CloseFilterPanel()
    {
        var animation = new DoubleAnimation { From = 0, To = -320, Duration = TimeSpan.FromMilliseconds(250) };
        animation.Completed += (s, e) => Overlay.Visibility = Visibility.Collapsed;
        FilterPanelTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animation);
        _filterPanelOpen = false;
    }

    private void Overlay_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => CloseFilterPanel();

    private void ResetButton_Click(object sender, RoutedEventArgs e) => ResetAll();
    private void ResetFiltersButton_Click(object sender, RoutedEventArgs e) => ResetAll();

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
    {
        if (_filterPanelOpen) CloseFilterPanel();
        CitizenMenu.Visibility = Visibility.Visible;
    }

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("BrowseNeighborhood", _user));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, CitizenNavigationHelper.GetMembershipId(_user.Id) ?? 0, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("Neighborhoods", _user));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "Neighborhoods": CitizenMenu.Visibility = Visibility.Collapsed; break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("Neighborhoods", _user));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("Neighborhoods", _user));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
    private void NavigateToProfile()
        => CitizenNavigationHelper.NavigateToProfile(_user, this);

}
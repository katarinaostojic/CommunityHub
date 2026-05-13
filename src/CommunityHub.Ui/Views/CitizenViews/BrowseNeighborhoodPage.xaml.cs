using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class BrowseNeighborhoodPage : Window
{
    private readonly User _user;
    private readonly BrowseNeighborhoodViewModel _viewModel;
    private bool _filterPanelOpen = false;

    public BrowseNeighborhoodPage(User user)
    {
        InitializeComponent();
        _user = user;

        NeighborhoodService neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        _viewModel = new BrowseNeighborhoodViewModel(neighborhoodService);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        => _viewModel.Search(SearchTextBox.Text);

    private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        string? name = string.IsNullOrWhiteSpace(FilterNameTextBox.Text) ? null : FilterNameTextBox.Text.Trim();
        string? address = string.IsNullOrWhiteSpace(FilterAddressTextBox.Text) ? null : FilterAddressTextBox.Text.Trim();
        string? city = string.IsNullOrWhiteSpace(FilterCityTextBox.Text) ? null : FilterCityTextBox.Text.Trim();
        string? country = string.IsNullOrWhiteSpace(FilterCountryTextBox.Text) ? null : FilterCountryTextBox.Text.Trim();

        _viewModel.ApplyFilters(name, address, city, country);
        CloseFilterPanel();
    }

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
        Neighborhood neighborhood = (Neighborhood)((Button)sender).Tag;
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
        DoubleAnimation animation = new DoubleAnimation
        {
            From = -320,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(250)
        };
        FilterPanelTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animation);
        _filterPanelOpen = true;
    }

    private void CloseFilterPanel()
    {
        DoubleAnimation animation = new DoubleAnimation
        {
            From = 0,
            To = -320,
            Duration = TimeSpan.FromMilliseconds(250)
        };
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
        CitizenMenu.Visibility = Visibility.Collapsed;
        switch (destination)
        {
            case "Neighborhoods": break;
            case "MyRequests":
                new MyRequestsPage(_user).Show();
                Close();
                break;
            case "Events": MessageBox.Show("Go to Events page."); break;
            case "Citizens": MessageBox.Show("Go to Citizens page."); break;
            case "Meetings": MessageBox.Show("Go to Meetings page."); break;
            case "CityObjects": MessageBox.Show("Go to City Objects page."); break;
            case "Budget": MessageBox.Show("Go to Budget page."); break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
}
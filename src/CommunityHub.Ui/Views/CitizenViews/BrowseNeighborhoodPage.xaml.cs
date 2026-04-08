using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CommunityHub.Ui.Views.CitizenViews
{
    public partial class BrowseNeighborhoodPage : Window
    {
        private readonly NeighborhoodDbRepository _neighborhoodRepository;
        private readonly User _user;

        private List<Neighborhood> _allNeighborhoods = new();
        private List<Neighborhood> _filteredNeighborhoods = new();

        private bool _filterPanelOpen = false;

        public BrowseNeighborhoodPage(User user)
        {
            InitializeComponent();

            _neighborhoodRepository = new NeighborhoodDbRepository();
            _user = user;

            LoggedInUserTextBlock.Text = _user.Username;
            LoadNeighborhoods();
        }

        private void LoadNeighborhoods()
        {
            _allNeighborhoods = _neighborhoodRepository.SearchForCitizen(null, null, null, null);
            _filteredNeighborhoods = new List<Neighborhood>(_allNeighborhoods);
            DisplayNeighborhoods();
        }

        private void DisplayNeighborhoods()
        {
            ResultsTitleTextBlock.Text = $"Browse Neighborhood - {_filteredNeighborhoods.Count} results";
            NeighborhoodsPanel.ItemsSource = _filteredNeighborhoods;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                _filteredNeighborhoods = new List<Neighborhood>(_allNeighborhoods);
                DisplayNeighborhoods();
                return;
            }

            int? number = null;
            string streetPart = search;

            string[] parts = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts[^1], out int parsedNumber))
            {
                number = parsedNumber;
                streetPart = string.Join(" ", parts.Take(parts.Length - 1));
            }

            _filteredNeighborhoods = _allNeighborhoods
                .Where(n =>
                    n.Name.ToLower().Contains(search) ||
                    n.Location.CityName.ToLower().Contains(search) ||
                    n.Location.CountryName.ToLower().Contains(search) ||
                    n.Streets.Any(s =>
                        s.StreetName.ToLower().Contains(search) ||
                        (
                            s.StreetName.ToLower().Contains(streetPart) &&
                            (!number.HasValue || (number.Value >= s.StartNumber && number.Value <= s.EndNumber))
                        )
                    )
                )
                .ToList();

            DisplayNeighborhoods();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            FilterNameTextBox.Text = string.Empty;
            FilterAddressTextBox.Text = string.Empty;
            FilterCityTextBox.Text = string.Empty;
            FilterCountryTextBox.Text = string.Empty;

            _filteredNeighborhoods = new List<Neighborhood>(_allNeighborhoods);
            DisplayNeighborhoods();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_filterPanelOpen)
                CloseFilterPanel();
            else
                OpenFilterPanel();
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
        {
            CloseFilterPanel();
        }

        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            string? name = string.IsNullOrWhiteSpace(FilterNameTextBox.Text) ? null : FilterNameTextBox.Text.Trim();
            string? address = string.IsNullOrWhiteSpace(FilterAddressTextBox.Text) ? null : FilterAddressTextBox.Text.Trim();
            string? city = string.IsNullOrWhiteSpace(FilterCityTextBox.Text) ? null : FilterCityTextBox.Text.Trim();
            string? country = string.IsNullOrWhiteSpace(FilterCountryTextBox.Text) ? null : FilterCountryTextBox.Text.Trim();

            _filteredNeighborhoods = _neighborhoodRepository.SearchForCitizen(name, address, city, country);

            DisplayNeighborhoods();
            CloseFilterPanel();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            FilterNameTextBox.Text = string.Empty;
            FilterAddressTextBox.Text = string.Empty;
            FilterCityTextBox.Text = string.Empty;
            FilterCountryTextBox.Text = string.Empty;

            _filteredNeighborhoods = new List<Neighborhood>(_allNeighborhoods);
            DisplayNeighborhoods();
        }

        private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
        {
            Neighborhood neighborhood = (Neighborhood)((Button)sender).Tag;

            MessageBox.Show(
                $"Selected neighborhood: {neighborhood.Name}",
                "Request Access",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void MyRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("My Requests page.");
        }
    }
}
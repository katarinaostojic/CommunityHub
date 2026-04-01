using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CommunityHub.Ui.Views.CitizenViews
{
    public partial class BrowseNeighborhoodPage : Window
    {
        private readonly NeighborhoodDbRepository _neighborhoodRepository;
        private readonly User _user;

        private List<Neighborhood> _allNeighborhoods = new();
        private List<Neighborhood> _filteredNeighborhoods = new();

        private int _currentPage = 1;
        private const int PageSize = 3;
        private bool _filterPanelOpen = false;

        public BrowseNeighborhoodPage(User user)
        {
            InitializeComponent();
            _neighborhoodRepository = new NeighborhoodDbRepository();
            _user = user;
            LoadNeighborhoods();
        }

        private void LoadNeighborhoods()
        {
            _allNeighborhoods = _neighborhoodRepository.Search(null, null, null, null);
            _filteredNeighborhoods = _allNeighborhoods;
            _currentPage = 1;
            DisplayNeighborhoods();
        }

        private void DisplayNeighborhoods()
        {
            int totalPages = (int)Math.Ceiling(_filteredNeighborhoods.Count / (double)PageSize);
            if (totalPages == 0)
                totalPages = 1;

            PageLabel.Text = $"Page {_currentPage} of {totalPages}";

            NeighborhoodsPanel.ItemsSource = _filteredNeighborhoods
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(search))
            {
                _filteredNeighborhoods = _allNeighborhoods;
            }
            else
            {
                _filteredNeighborhoods = _neighborhoodRepository.Search(search, null, null, null);
            }

            _currentPage = 1;
            DisplayNeighborhoods();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            FilterNameTextBox.Text = string.Empty;
            FilterCityTextBox.Text = string.Empty;

            _filteredNeighborhoods = _allNeighborhoods;
            _currentPage = 1;
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

        private void Overlay_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CloseFilterPanel();
        }

        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            string? name = string.IsNullOrWhiteSpace(FilterNameTextBox.Text)
                ? null
                : FilterNameTextBox.Text.Trim();

            string? city = string.IsNullOrWhiteSpace(FilterCityTextBox.Text)
                ? null
                : FilterCityTextBox.Text.Trim();

            _filteredNeighborhoods = _neighborhoodRepository.Search(name, city, null, null);

            _currentPage = 1;
            DisplayNeighborhoods();
            CloseFilterPanel();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            FilterNameTextBox.Text = string.Empty;
            FilterCityTextBox.Text = string.Empty;

            _filteredNeighborhoods = _allNeighborhoods;
            _currentPage = 1;
            DisplayNeighborhoods();
        }

        private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
        {
            Neighborhood neighborhood = (Neighborhood)((Button)sender).Tag;

            MessageBox.Show(
                $"Request for neighborhood '{neighborhood.Name}' has been sent.",
                "Request Sent",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                DisplayNeighborhoods();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling(_filteredNeighborhoods.Count / (double)PageSize);
            if (totalPages == 0)
                totalPages = 1;

            if (_currentPage < totalPages)
            {
                _currentPage++;
                DisplayNeighborhoods();
            }
        }

        private void MyRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ovde otvori prozor za moje zahteve.");
        }
    }
}
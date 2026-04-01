using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CommunityHub.Ui.Views;

//za putanje za slike
public class FirstImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<AppImage> images && images.Count > 0)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                images[0].Path.Replace('/', Path.DirectorySeparatorChar)
            );
            try { return new BitmapImage(new Uri(fullPath, UriKind.Absolute)); }
            catch { return null; }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public partial class BrowseBuildingsPage : Page
{
    private readonly BuildingDbRepository _buildingRepository;
    private readonly User _user;
    private List<Building> _allBuildings;
    private List<Building> _filteredBuildings;
    private int _currentPage = 1;
    private const int PageSize = 3;
    private bool _filterPanelOpen = false;

    public BrowseBuildingsPage(User user)
    {
        InitializeComponent();
        _buildingRepository = new BuildingDbRepository();
        _user = user;
        LoadBuildings();
    }

    private void LoadBuildings()
    {
        _allBuildings = _buildingRepository.GetAll();
        _filteredBuildings = _allBuildings;
        _currentPage = 1;
        DisplayBuildings();
    }

    private void DisplayBuildings()
    {
        int totalPages = (int)Math.Ceiling(_filteredBuildings.Count / (double)PageSize);
        if (totalPages == 0) totalPages = 1;
        PageLabel.Text = $"Page {_currentPage} of {totalPages}";

        BuildingsPanel.ItemsSource = _filteredBuildings
            .Skip((_currentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        Search();
    }

    private void Search()
    {
        string search = SearchTextBox.Text.Trim();
        _filteredBuildings = string.IsNullOrEmpty(search)
            ? _allBuildings
            : _buildingRepository.Search(search, null, null, null);
        _currentPage = 1;
        DisplayBuildings();
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
        string? street = string.IsNullOrWhiteSpace(FilterStreetTextBox.Text) ? null : FilterStreetTextBox.Text.Trim();
        string? neighborhood = string.IsNullOrWhiteSpace(FilterNeighborhoodTextBox.Text) ? null : FilterNeighborhoodTextBox.Text.Trim();
        string? city = string.IsNullOrWhiteSpace(FilterCityTextBox.Text) ? null : FilterCityTextBox.Text.Trim();
        string? country = string.IsNullOrWhiteSpace(FilterCountryTextBox.Text) ? null : FilterCountryTextBox.Text.Trim();

        if (street == null && neighborhood == null && city == null && country == null)
            _filteredBuildings = _allBuildings;
        else
            _filteredBuildings = _buildingRepository.Search(street, neighborhood, city, country);

        _currentPage = 1;
        DisplayBuildings();
        CloseFilterPanel();
    }

    private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        FilterStreetTextBox.Text = string.Empty;
        FilterNeighborhoodTextBox.Text = string.Empty;
        FilterCityTextBox.Text = string.Empty;
        FilterCountryTextBox.Text = string.Empty;
        _filteredBuildings = _allBuildings;
        _currentPage = 1;
        DisplayBuildings();
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Text = string.Empty;
        FilterStreetTextBox.Text = string.Empty;
        FilterNeighborhoodTextBox.Text = string.Empty;
        FilterCityTextBox.Text = string.Empty;
        FilterCountryTextBox.Text = string.Empty;
        _filteredBuildings = _allBuildings;
        _currentPage = 1;
        DisplayBuildings();
    }

    private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
    {
        Building building = (Building)((Button)sender).Tag;
        // NavigationService.Navigate(new RequestAccessPage(building, _user));
    }

    private void PrevPageButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            DisplayBuildings();
        }
    }

    private void NextPageButton_Click(object sender, RoutedEventArgs e)
    {
        int totalPages = (int)Math.Ceiling(_filteredBuildings.Count / (double)PageSize);
        if (_currentPage < totalPages)
        {
            _currentPage++;
            DisplayBuildings();
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void MyRequestsButton_Click(object sender, RoutedEventArgs e)
    {
    }
}
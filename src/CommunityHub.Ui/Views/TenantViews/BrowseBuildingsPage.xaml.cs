using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CommunityHub.Ui.Views;

public class FirstImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<string> paths && paths.Count > 0)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                paths[0].Replace('/', Path.DirectorySeparatorChar)
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

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Text = string.Empty;
        _filteredBuildings = _allBuildings;
        _currentPage = 1;
        DisplayBuildings();
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
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
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerNoticeBoardDetailsPage : Page
{
    private readonly User _currentUser;
    private readonly BuildingDto _building;
    private readonly AdService _adService;
    private List<Ad> _allAds = new();
    private bool _isYearMode = true;

    public ManagerNoticeBoardDetailsPage(User user, BuildingDto building)
    {
        InitializeComponent();
        _currentUser = user;
        _building = building;
        _adService = Injector.CreateInstance<AdService>();
        BuildingTitleText.Text = $"{_building.Street} {_building.StreetNumber}";
        LoadAds();
        PopulatePeriodComboBox();
    }

    private void LoadAds()
    {
        _allAds = _adService.GetAllByBuilding(_building.Id);
        AdsItemsControl.ItemsSource = _allAds
            .Select(a => new ManagerAdViewModel(a))
            .ToList();
    }

    private void PopulatePeriodComboBox()
    {
        PeriodComboBox.Items.Clear();
        if (_isYearMode)
        {
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
                PeriodComboBox.Items.Add(year);
        }
        else
        {
            string[] months = { "January", "February", "March", "April", "May", "June",
                                 "July", "August", "September", "October", "November", "December" };
            foreach (string month in months)
                PeriodComboBox.Items.Add(month);
        }
        PeriodComboBox.SelectedIndex = 0;
    }

    private void LoadStatistics()
    {
        List<Ad> filteredAds = GetFilteredAds();

        TxtOffering.Text = $"Offering help: {_adService.CountByType(filteredAds, AdType.Offering)}";
        TxtSeeking.Text = $"Seeking help: {_adService.CountByType(filteredAds, AdType.Seeking)}";

        var categoryStats = _adService.GetStatsByCategory(filteredAds);
        CategoryStatsItemsControl.ItemsSource = categoryStats
            .Select(kvp => new
            {
                Category = kvp.Key.ToString(),
                Offering = kvp.Value.offering,
                Seeking = kvp.Value.seeking
            }).ToList();

        var (active, archived) = _adService.GetCurrentState(_allAds);
        TxtActive.Text = $"Active ads: {active}";
        TxtArchived.Text = $"Archived ads: {archived}";

        var activeByCategory = _adService.GetActiveCountByCategory(_allAds);
        ActiveByCategoryItemsControl.ItemsSource = activeByCategory
            .Select(kvp => new { Display = $"{kvp.Key}: {kvp.Value}" })
            .ToList();

        User? topHelper = _adService.GetTopHelper(_building.Id);
        TxtTopHelper.Text = topHelper != null
            ? $"{topHelper.Name} {topHelper.Surname}"
            : "No data yet";
    }

    private List<Ad> GetFilteredAds()
    {
        if (PeriodComboBox.SelectedItem == null) return _allAds;

        if (_isYearMode)
        {
            int selectedYear = (int)PeriodComboBox.SelectedItem;
            return _allAds
                .Where(a => a.DateFrom.Year == selectedYear || a.DateTo.Year == selectedYear)
                .ToList();
        }
        else
        {
            int selectedMonth = PeriodComboBox.SelectedIndex + 1;
            return _allAds
                .Where(a => a.DateFrom.Month == selectedMonth || a.DateTo.Month == selectedMonth)
                .ToList();
        }
    }

    private void TabAds_Click(object sender, RoutedEventArgs e)
    {
        PanelAds.Visibility = Visibility.Visible;
        PanelStatistics.Visibility = Visibility.Collapsed;
        SetActiveTab(TabAds);
        SetInactiveTab(TabStatistics);
    }

    private void TabStatistics_Click(object sender, RoutedEventArgs e)
    {
        PanelAds.Visibility = Visibility.Collapsed;
        PanelStatistics.Visibility = Visibility.Visible;
        SetActiveTab(TabStatistics);
        SetInactiveTab(TabAds);
        LoadStatistics();
    }

    private void BtnYear_Click(object sender, RoutedEventArgs e)
    {
        _isYearMode = true;
        BtnYear.Background = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        BtnYear.Foreground = Brushes.White;
        BtnMonth.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        BtnMonth.Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80));
        PopulatePeriodComboBox();
        LoadStatistics();
    }

    private void BtnMonth_Click(object sender, RoutedEventArgs e)
    {
        _isYearMode = false;
        BtnMonth.Background = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        BtnMonth.Foreground = Brushes.White;
        BtnYear.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        BtnYear.Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80));
        PopulatePeriodComboBox();
        LoadStatistics();
    }

    private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PanelStatistics.Visibility == Visibility.Visible)
            LoadStatistics();
    }

    private void SetActiveTab(Button tab)
    {
        tab.Foreground = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        tab.BorderThickness = new Thickness(0, 0, 0, 3);
        tab.BorderBrush = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        tab.FontWeight = FontWeights.SemiBold;
    }

    private void SetInactiveTab(Button tab)
    {
        tab.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
        tab.BorderThickness = new Thickness(0);
        tab.FontWeight = FontWeights.Normal;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}
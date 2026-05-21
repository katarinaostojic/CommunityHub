using CommunityHub.Application.Domain.Shared;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerNoticeBoardDetailsPage : Page
{
    private readonly User _currentUser;
    private readonly ManagerNoticeBoardDetailsViewModel _viewModel;
    private bool _isYearMode = true;
    private int _selectedYear = DateTime.Now.Year;
    private int _selectedMonth = 1;

    public ManagerNoticeBoardDetailsPage(User user, long buildingId, string buildingTitle)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new ManagerNoticeBoardDetailsViewModel(buildingId, buildingTitle);
        DataContext = _viewModel;
        PopulatePeriodComboBox();
    }

    private void PopulatePeriodComboBox()
    {
        PeriodComboBox.Items.Clear();
        if (_isYearMode)
        {
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
                PeriodComboBox.Items.Add(year);
            PeriodComboBox.SelectedItem = _selectedYear;
        }
        else
        {
            string[] months = { "January", "February", "March", "April", "May", "June",
                                 "July", "August", "September", "October", "November", "December" };
            foreach (string month in months)
                PeriodComboBox.Items.Add(month);
            PeriodComboBox.SelectedIndex = _selectedMonth - 1;
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

    private void LoadStatistics()
    {
        if (_isYearMode && PeriodComboBox.SelectedItem is int year)
        {
            _selectedYear = year;
            _viewModel.LoadStatistics(year, null);
        }
        else if (!_isYearMode && PeriodComboBox.SelectedIndex >= 0)
        {
            _selectedMonth = PeriodComboBox.SelectedIndex + 1;
            _viewModel.LoadStatistics(_selectedYear, _selectedMonth);
        }
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
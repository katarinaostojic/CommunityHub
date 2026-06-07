using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class StatisticsDialog : Window
{
    private readonly CityObjectsViewModel _viewModel;

    public StatisticsDialog(CityObjectsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        LoadStatistics();
    }

    private void LoadStatistics()
    {
        bool isMonthly = MonthlyRadio.IsChecked == true;
        int? month = isMonthly ? DateTime.Now.Month : null;
        int? year = DateTime.Now.Year;

        var stats = _viewModel.GetStatistics(month, year);
        TotalReservationsText.Text = stats.TotalReservations.ToString();
        StatisticsList.ItemsSource = stats.VisitCounts;
    }

    private void PeriodRadio_Checked(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded) return;
        LoadStatistics();
    }

    private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        CityObjectVisitCountDto item = (CityObjectVisitCountDto)((Button)sender).Tag;
        bool isMonthly = MonthlyRadio.IsChecked == true;
        int? month = isMonthly ? DateTime.Now.Month : null;
        int? year = DateTime.Now.Year;
        var history = _viewModel.GetHistoryById(item.CityObjectId, month, year);
        HistoryDialog dialog = new HistoryDialog(item.CityObjectName, history);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => Close();
}

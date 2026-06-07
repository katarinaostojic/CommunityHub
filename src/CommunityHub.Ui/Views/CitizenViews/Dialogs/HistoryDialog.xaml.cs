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

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class HistoryDialog : Window
{
    public HistoryDialog(string cityObjectName, List<CityObjectReservationDto> history)
    {
        InitializeComponent();
        TitleText.Text = cityObjectName;
        TotalVisitsText.Text = history.Count.ToString();
        HistoryList.ItemsSource = history;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => Close();
}

using CommunityHub.Application.Domain;
using System;
using System.Windows;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class NeighborhoodAccessGrantedDialog : Window
{
    private readonly User _user;
    private readonly Window _parentWindow;

    public NeighborhoodAccessGrantedDialog(User user, string neighborhoodName, Window parentWindow)
    {
        InitializeComponent();
        _user = user;
        _parentWindow = parentWindow;

        NeighborhoodNameText.Text = neighborhoodName;
        CitizenNameText.Text = _user.Username;
        JoinDateText.Text = DateTime.Now.ToString("dd.MM.yyyy.");

        CloseButton.Click += CloseButton_Click;
        GoToDashboardButton.Click += GoToDashboardButton_Click;
        ViewMyRequestsButton.Click += ViewMyRequestsButton_Click;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void GoToDashboardButton_Click(object sender, RoutedEventArgs e) => Close();

    private void ViewMyRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        new MyRequestsPage(_user).Show();
        _parentWindow?.Close();
        Close();
    }
}
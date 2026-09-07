using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Ui.Helpers.Citizen;
using System;
using System.Windows;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class NeighborhoodRequestCreatedDialog : Window
{
    private readonly User _user;

    public NeighborhoodRequestCreatedDialog(User user, string neighborhoodName)
    {
        InitializeComponent();
        _user = user;

        NeighborhoodNameText.Text = neighborhoodName;
        CitizenNameText.Text = _user.Username;
        CreatedOnText.Text = DateTime.Now.ToString("dd.MM.yyyy.");
        StatusText.Text = ResourceHelper.Get("Status_Pending", "Pending approval");

        CloseButton.Click += CloseButton_Click;
        GoToDashboardButton.Click += GoToDashboardButton_Click;
        ViewMyRequestsButton.Click += ViewMyRequestsButton_Click;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void GoToDashboardButton_Click(object sender, RoutedEventArgs e) => Close();

    private void ViewMyRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        new MyRequestsPage(_user).Show();
        Close();
    }
}

using CommunityHub.Application.Domain;
using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerMainWindow : Window
{
    private readonly User _currentUser;

    public ManagerMainWindow(User user)
    {
        InitializeComponent();
        _currentUser = user;
        MainFrame.Navigate(new MyBuildingsPage(_currentUser));
    }

    private void BuildingsButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new MyBuildingsPage(_currentUser));
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        new LogInForm().Show();
        Close();
    }
}
using CommunityHub.Application.Domain.Entities.Shared;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerMainWindow : Window
{
    private readonly User _currentUser;

    public ManagerMainWindow(User user)
    {
        InitializeComponent();
        _currentUser = user;
        MainFrame.Navigate(new MyBuildingsPage(_currentUser));
        SetActiveNavButton(BtnBuildings);
    }

    private void BuildingsButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnBuildings);
        MainFrame.Navigate(new MyBuildingsPage(_currentUser));
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        new LogInForm().Show();
        Close();
    }

    private void AccessRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnAccessRequests);
        MainFrame.Navigate(new AccessRequestsPage(_currentUser));
    }

    public void NavigateToBuildings()
    {
        MainFrame.Navigate(new MyBuildingsPage(_currentUser));
    }

    private void SetActiveNavButton(Button activeButton)
    {
        var navButtons = new[] { BtnBuildings, BtnAccessRequests, BtnNoticeboard, BtnAssembly, BtnProblems };
        foreach (var btn in navButtons)
        {
            btn.Background = Brushes.Transparent;
            btn.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
        }
        activeButton.Background = new SolidColorBrush(Color.FromRgb(44, 62, 80));
        activeButton.Foreground = Brushes.White;
    }

    private void NoticeboardButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnNoticeboard);
        MainFrame.Navigate(new ManagerNoticeBoardPage(_currentUser));
    }
}
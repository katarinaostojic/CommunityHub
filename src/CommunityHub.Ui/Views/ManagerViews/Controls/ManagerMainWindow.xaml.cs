using CommunityHub.Application.Domain.Entities.Shared;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityHub.Ui.Helpers.Manager;

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
        UpdateTooltips();
    }

    private void TooltipsButton_Click(object sender, RoutedEventArgs e)
    {
        AppSession.IsTooltipsEnabled = !AppSession.IsTooltipsEnabled;
        TooltipsText.Text = AppSession.IsTooltipsEnabled ? "  Tooltips: ON" : "  Tooltips: OFF";
        UpdateTooltips();
    }

    private void UpdateTooltips()
    {
        var buttons = new[] { BtnBuildings, BtnAccessRequests, BtnNoticeboard, BtnResidentMeeting, BtnProblems };
        foreach (var btn in buttons)
            ToolTipService.SetIsEnabled(btn, AppSession.IsTooltipsEnabled);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: navigate to profile
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
        var navButtons = new[] { BtnBuildings, BtnAccessRequests, BtnNoticeboard, BtnResidentMeeting, BtnProblems };
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

    private void ProblemsButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnProblems);
        MainFrame.Navigate(new Buildings.Problems.ProblemsPage(_currentUser));
    }

    private void ResidentMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnResidentMeeting);
        // TODO: navigate to ResidentMeeting page
    }
}
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Helpers.Manager;
using CommunityHub.Ui.Helpers.Manager.Onboarding;
using CommunityHub.Ui.Views.ManagerViews.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerMainWindow : Window
{
    private readonly User _currentUser;
    private BuildingDto? _lastSelectedBuildingProblems;
    private BuildingDto? _lastSelectedBuildingMeetings;

    public ManagerMainWindow(User user)
    {
        InitializeComponent();
        _currentUser = user;
        MainFrame.Navigate(new MyBuildingsPage(_currentUser));
        SetActiveNavButton(BtnBuildings);
        UpdateTooltips();

        MainFrame.Navigated += (s, e) => UpdateTooltips();

        if (!OnboardingState.HasSeenWizard())
        {
            Loaded += ShowOnboardingWizard;
        }
    }

    private void ShowOnboardingWizard(object sender, RoutedEventArgs e)
    {
        Loaded -= ShowOnboardingWizard;

        ManagerOnboardingWizard wizard = new ManagerOnboardingWizard { Owner = this };
        wizard.ShowDialog();
    }

    private void TooltipsButton_Click(object sender, RoutedEventArgs e)
    {
        AppSession.IsTooltipsEnabled = !AppSession.IsTooltipsEnabled;
        TooltipsText.Text = AppSession.IsTooltipsEnabled ? "  Tooltips: ON" : "  Tooltips: OFF";
        UpdateTooltips();
    }

    private void UpdateTooltips()
    {
        TooltipsManager.Apply(this);
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
        var navButtons = new[] { BtnBuildings, BtnAccessRequests, BtnNoticeboard, BtnResidentMeeting, BtnProblems, BtnHelp };
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

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnHelp);
        MainFrame.Navigate(new ManagerHelpPage());
    }

    private void ProblemsButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnProblems);
        var page = new Buildings.Problems.ProblemsPage(_currentUser, _lastSelectedBuildingProblems);
        page.BuildingSelected += building => _lastSelectedBuildingProblems = building;
        MainFrame.Navigate(page);
    }

    private void ResidentMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        SetActiveNavButton(BtnResidentMeeting);
        var page = new Buildings.ResidentMeetings.ResidentMeetingsPage(_currentUser, _lastSelectedBuildingMeetings);
        page.BuildingSelected += building => _lastSelectedBuildingMeetings = building;
        MainFrame.Navigate(page);
    }
}
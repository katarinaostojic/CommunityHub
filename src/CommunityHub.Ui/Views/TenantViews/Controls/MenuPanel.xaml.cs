using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Views.TenantViews;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace CommunityHub.Ui.Controls;

public partial class MenuPanel : UserControl
{
    private readonly BuildingService _buildingService;
    private User _user;
    private readonly BuildingMembershipService _membershipService;

    public MenuPanel()
    {
        InitializeComponent();
        _buildingService = new BuildingService();
        _membershipService = new BuildingMembershipService();
    }

    public void Initialize(User user)
    {
        _user = user;
        // Reset My Buildings toggle state on each navigation
        MyBuildingsToggle.IsChecked = false;
        MyBuildingsScrollViewer.Visibility = Visibility.Collapsed;
        MyBuildingsArrow.Text = "∨";
        LoadMemberships();
    }

    public void Open()
    {
        MenuOverlay.Visibility = Visibility.Visible;
        DoubleAnimation animation = new DoubleAnimation
        {
            From = -300,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        MenuPanelTranslate.BeginAnimation(TranslateTransform.XProperty, animation);
    }

    public void Close()
    {
        DoubleAnimation animation = new DoubleAnimation
        {
            From = 0,
            To = -300,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        animation.Completed += (s, e) => MenuOverlay.Visibility = Visibility.Collapsed;
        MenuPanelTranslate.BeginAnimation(TranslateTransform.XProperty, animation);
    }

    private void LoadMemberships()
    {
        var memberships = _membershipService.GetByTenant(_user.Id);
        MyBuildingsMenuPanel.ItemsSource = memberships;
    }

    private void MenuOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Close();
    }

    private void BrowseBuildingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
        NavigationService.GetNavigationService(this)?.Navigate(new BrowseBuildingsPage(_user));
    }

    private void MyRequestsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
        NavigationService.GetNavigationService(this)?.Navigate(new MyBuildingRequestsPage(_user));
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
        Window mainWindow = Window.GetWindow(this);
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        mainWindow?.Close();
    }

    private void CloseMenuButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MyBuildingsToggle_Checked(object sender, RoutedEventArgs e)
    {
        MyBuildingsScrollViewer.Visibility = Visibility.Visible;
        MyBuildingsArrow.Text = "▲";
    }

    private void MyBuildingsToggle_Unchecked(object sender, RoutedEventArgs e)
    {
        MyBuildingsScrollViewer.Visibility = Visibility.Collapsed;
        MyBuildingsArrow.Text = "▼";
    }

    private void BuildingName_Click(object sender, MouseButtonEventArgs e)
    {
        BuildingMembership membership = (BuildingMembership)((Border)sender).Tag;
        Close();
        NavigationService.GetNavigationService(this)?.Navigate(new BuildingDetailsPage(membership.Building, _user));
    }
}
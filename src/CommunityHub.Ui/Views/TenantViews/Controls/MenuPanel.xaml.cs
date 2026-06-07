using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Views.TenantViews;
using CommunityHub.Ui.Views.TenantViews.Buildings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace CommunityHub.Ui.Controls;

public partial class MenuPanel : UserControl
{
    private User _user;
    private readonly MenuPanelViewModel _viewModel;
    private readonly BuildingService _buildingService;

    public MenuPanel()
    {
        InitializeComponent();
        BuildingMembershipService membershipService = Injector.CreateInstance<BuildingMembershipService>();
        _buildingService = Injector.CreateInstance<BuildingService>();
        _viewModel = new MenuPanelViewModel(membershipService);
    }

    public void Initialize(User user)
    {
        _user = user;
        MyBuildingsToggle.IsChecked = false;
        MyBuildingsScrollViewer.Visibility = Visibility.Collapsed;
        MyBuildingsArrow.Text = "▼";
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
        MyBuildingsMenuPanel.ItemsSource = _viewModel.GetMemberships(_user.Id);
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
        BuildingMembershipDto membership = (BuildingMembershipDto)((Border)sender).Tag;
        var fullDto = _buildingService.GetById(membership.BuildingId);
        if (fullDto == null) return;
        Close();
        NavigationService.GetNavigationService(this)?.Navigate(new BuildingDetailsPage(fullDto, _user));
    }

    private void NoticeBoardMenuItem_Click(object sender, RoutedEventArgs e)
    {
        BuildingMembershipDto membership = (BuildingMembershipDto)((Button)sender).Tag;
        Close();
        NavigationService.GetNavigationService(this)?.Navigate(new NoticeBoardPage(_user, membership));
    }

    private void CommonRoomsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        BuildingMembershipDto membership = (BuildingMembershipDto)((Button)sender).Tag;
        string buildingInfo = $"{membership.BuildingStreet} {membership.BuildingStreetNumber}, {membership.BuildingNeighborhood}";
        Close();
        NavigationService.GetNavigationService(this)?.Navigate(
            new CommonRoomsPage(_user, membership.BuildingId, buildingInfo));
    }

    private void ReportedProblemsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        BuildingMembershipDto membership = (BuildingMembershipDto)((Button)sender).Tag;
        Close();

        NavigationService.GetNavigationService(this)?.Navigate(
            new ReportedProblemsPage(_user, membership));
    }

    private void ResidentsMeetingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        BuildingMembershipDto membership = (BuildingMembershipDto)((Button)sender).Tag;
        Close();

        NavigationService.GetNavigationService(this)?.Navigate(
            new ResidentsMeetingsPage(_user, membership));
    }
}
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.Ads;
using CommunityHub.Ui.ViewModels.TenantViewModels.Notifications;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews.Notifications;

public partial class NotificationsPage : Page
{
    private readonly User _user;
    private readonly Page? _returnPage;
    private readonly NotificationsPageViewModel _viewModel;
    private readonly BuildingMembershipService _membershipService;

    public NotificationsPage(User user, Page? returnPage = null)
    {
        InitializeComponent();

        _user = user;
        _returnPage = returnPage;

        AdNotificationService notificationService = Injector.CreateInstance<AdNotificationService>();
        _membershipService = Injector.CreateInstance<BuildingMembershipService>();

        _viewModel = new NotificationsPageViewModel(notificationService, user.Id);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        AppMenu.Initialize(_user);
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        if (_returnPage != null)
        {
            NavigationService.Navigate(_returnPage);
            return;
        }

        if (NavigationService.CanGoBack)
            NavigationService.GoBack();
    }

    private void MarkAllAsReadButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.MarkAllAsRead();
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.DataContext is not NotificationItemViewModel notification)
            return;

        _viewModel.MarkAsRead(notification.Id);

        BuildingMembershipDto? membership = GetMembership(notification.Ad.BuildingId);

        if (membership == null)
        {
            MessageBox.Show("Building membership was not found.");
            return;
        }

        Page page = CreateNotificationTargetPage(notification, membership);
        NavigationService.Navigate(page);
    }

    private Page CreateNotificationTargetPage(
        NotificationItemViewModel notification,
        BuildingMembershipDto membership)
    {
        if (notification.Type == AdNotificationType.MatchingAd)
        {
            return new BookSlotsPage(
                _user,
                membership,
                notification.RelatedAd,
                notification.Ad,
                this);
        }

        return new AdDetailsPage(_user, membership, notification.Ad);
    }

    private BuildingMembershipDto? GetMembership(long buildingId)
    {
        return _membershipService
            .GetByTenant(_user.Id)
            .FirstOrDefault(membership => membership.BuildingId == buildingId);
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }
}
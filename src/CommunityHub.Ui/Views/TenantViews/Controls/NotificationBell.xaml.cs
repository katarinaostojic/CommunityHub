using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Ads;
using CommunityHub.Ui.Views.TenantViews.Notifications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CommunityHub.Ui.Controls;

public partial class NotificationBell : UserControl
{
    private User? _user;

    public NotificationBell()
    {
        InitializeComponent();
    }

    public void Initialize(User user)
    {
        _user = user;
        LoadUnreadCount(user.Id);
    }

    public void Refresh()
    {
        if (_user == null)
            return;

        LoadUnreadCount(_user.Id);
    }

    private void LoadUnreadCount(long userId)
    {
        try
        {
            AdNotificationService notificationService =
                Injector.CreateInstance<AdNotificationService>();

            int unreadCount = notificationService
                .GetUnreadNotifications(userId)
                .Count;

            ShowUnreadCount(unreadCount);
        }
        catch
        {
            ShowUnreadCount(0);
        }
    }

    private void ShowUnreadCount(int unreadCount)
    {
        if (unreadCount <= 0)
        {
            BadgeBorder.Visibility = Visibility.Collapsed;
            return;
        }

        BadgeTextBlock.Text = unreadCount > 99 ? "99+" : unreadCount.ToString();
        BadgeBorder.Visibility = Visibility.Visible;
    }

    private void BellButton_Click(object sender, RoutedEventArgs e)
    {
        if (_user == null)
            return;

        NavigationService? navigationService = NavigationService.GetNavigationService(this);
        navigationService?.Navigate(new NotificationsPage(_user));
    }
}
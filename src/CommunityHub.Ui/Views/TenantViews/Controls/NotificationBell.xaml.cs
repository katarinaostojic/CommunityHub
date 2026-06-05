using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Ads;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Controls;

public partial class NotificationBell : UserControl
{
    public NotificationBell()
    {
        InitializeComponent();
    }

    public void Initialize(long userId)
    {
        LoadUnreadCount(userId);
    }

    public void Refresh(long userId)
    {
        LoadUnreadCount(userId);
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
}
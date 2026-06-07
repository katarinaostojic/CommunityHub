using CommunityHub.Application.Services.Entities.Buildings.Ads;
using CommunityHub.Ui.ViewModels;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Notifications;

public class NotificationsPageViewModel : BaseViewModel
{
    private readonly AdNotificationService _notificationService;
    private readonly long _userId;

    private ObservableCollection<NotificationGroupViewModel> _groups = new();
    private bool _hasUnreadNotifications;

    public NotificationsPageViewModel(AdNotificationService notificationService, long userId)
    {
        _notificationService = notificationService;
        _userId = userId;

        Load();
    }

    public ObservableCollection<NotificationGroupViewModel> Groups
    {
        get => _groups;
        private set => SetProperty(ref _groups, value);
    }

    public bool HasUnreadNotifications
    {
        get => _hasUnreadNotifications;
        private set => SetProperty(ref _hasUnreadNotifications, value);
    }

    public bool HasNotifications => Groups.Any(group => group.Notifications.Any());

    public bool HasNoNotifications => !HasNotifications;

    public void MarkAllAsRead()
    {
        if (!HasUnreadNotifications)
            return;

        _notificationService.MarkAllAsRead(_userId);

        foreach (NotificationItemViewModel notification in Groups.SelectMany(group => group.Notifications))
            notification.MarkAsRead();

        HasUnreadNotifications = false;
    }

    public void ClearAll()
    {
        if (!HasNotifications)
            return;

        _notificationService.ClearAll(_userId);
        Groups.Clear();
        HasUnreadNotifications = false;
        NotifyNotificationAvailabilityChanged();
    }

    public void MarkAsRead(long notificationId)
    {
        NotificationItemViewModel? notification = Groups
            .SelectMany(group => group.Notifications)
            .FirstOrDefault(item => item.Id == notificationId);

        if (notification == null || notification.IsRead)
            return;

        _notificationService.MarkNotificationAsRead(notificationId);
        notification.MarkAsRead();

        HasUnreadNotifications = Groups
            .SelectMany(group => group.Notifications)
            .Any(item => item.IsUnread);
    }

    private void Load()
    {
        List<NotificationItemViewModel> notifications = _notificationService
            .GetNotifications(_userId)
            .Select(notification => new NotificationItemViewModel(notification))
            .OrderByDescending(notification => notification.CreatedAt)
            .ToList();

        Groups = new ObservableCollection<NotificationGroupViewModel>(
            notifications
                .GroupBy(notification => notification.CreatedAt.Date)
                .OrderByDescending(group => group.Key)
                .Select(group => new NotificationGroupViewModel(
                    GetGroupTitle(group.Key),
                    group)));

        HasUnreadNotifications = notifications.Any(notification => notification.IsUnread);
        NotifyNotificationAvailabilityChanged();
    }

    private void NotifyNotificationAvailabilityChanged()
    {
        OnPropertyChanged(nameof(HasNotifications));
        OnPropertyChanged(nameof(HasNoNotifications));
    }

    private static string GetGroupTitle(DateTime date)
    {
        DateTime today = DateTime.Today;

        if (date == today)
            return "TODAY";

        if (date == today.AddDays(-1))
            return "YESTERDAY";

        return date.ToString("dd.MM.yyyy").ToUpper();
    }
}
using CommunityHub.Application.Services.Interfaces.Ads;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NoticeBoard;

public class NoticeBoardNotificationsViewModel : BaseViewModel
{
    private readonly IAdNotificationService _notificationService;
    private readonly long _currentUserId;

    private ObservableCollection<AdNotificationViewModel> _items = new();
    private bool _hasItems;

    public NoticeBoardNotificationsViewModel(
        IAdNotificationService notificationService,
        long currentUserId)
    {
        _notificationService = notificationService;
        _currentUserId = currentUserId;

        Load();
    }

    public ObservableCollection<AdNotificationViewModel> Items
    {
        get => _items;
        private set => SetProperty(ref _items, value);
    }

    public bool HasItems
    {
        get => _hasItems;
        private set => SetProperty(ref _hasItems, value);
    }

    public void Dismiss(long notificationId)
    {
        AdNotificationViewModel? notification = Items
            .FirstOrDefault(n => n.Id == notificationId);

        if (notification == null) return;

        _notificationService.MarkNotificationAsRead(notificationId);

        Items.Remove(notification);
        HasItems = Items.Count > 0;
    }

    private void Load()
    {
        List<AdNotificationViewModel> notifications = _notificationService
            .GetUnreadNotifications(_currentUserId)
            .Select(n => new AdNotificationViewModel(n))
            .ToList();

        Items = new ObservableCollection<AdNotificationViewModel>(notifications);
        HasItems = notifications.Count > 0;
    }
}
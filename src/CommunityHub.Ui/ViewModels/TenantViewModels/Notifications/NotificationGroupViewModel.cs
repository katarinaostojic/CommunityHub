using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Notifications;

public class NotificationGroupViewModel
{
    public NotificationGroupViewModel(string title, IEnumerable<NotificationItemViewModel> notifications)
    {
        Title = title;
        Notifications = new ObservableCollection<NotificationItemViewModel>(notifications);
    }

    public string Title { get; }

    public ObservableCollection<NotificationItemViewModel> Notifications { get; }
}
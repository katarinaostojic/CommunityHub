using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;

namespace CommunityHub.Application.Services.Ads;

public class AdNotificationService
{
    private readonly IAdNotificationRepository _notificationRepository;

    public AdNotificationService(IAdNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public List<AdNotificationDto> GetUnreadNotifications(long userId)
    {
        return _notificationRepository
            .GetUnreadByUser(userId)
            .ToAdNotificationDtoList();
    }

    public void MarkNotificationAsRead(long notificationId)
    {
        _notificationRepository.MarkAsRead(notificationId);
    }

    public void MarkAllNotificationsAsRead(long userId)
    {
        _notificationRepository.MarkAllAsRead(userId);
    }
}
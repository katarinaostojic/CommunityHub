using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.Ads;
using CommunityHub.Application.DtoMappers.Buildings.Ads;
using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Application.Services.Entities.Buildings.Ads;

public class AdNotificationService
{
    private readonly IAdNotificationRepository _notificationRepository;

    public AdNotificationService(IAdNotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public List<AdNotificationDto> GetNotifications(long userId)
    {
        return _notificationRepository
            .GetByUser(userId)
            .ToAdNotificationDtoList();
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

    public void MarkAllAsRead(long userId)
    {
        _notificationRepository.MarkAllAsRead(userId);
    }
}
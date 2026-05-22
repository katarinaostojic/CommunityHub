using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Mappings.Ads;
using CommunityHub.Application.Services.Interfaces.Ads;

namespace CommunityHub.Application.Services.Entities.Ads;

public class AdNotificationService : IAdNotificationService
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
}
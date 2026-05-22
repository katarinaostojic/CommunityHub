using CommunityHub.Application.DTOs.Ads;

namespace CommunityHub.Application.Services.Interfaces.Ads;

public interface IAdNotificationService
{
    List<AdNotificationDto> GetUnreadNotifications(long userId);

    void MarkNotificationAsRead(long notificationId);
}
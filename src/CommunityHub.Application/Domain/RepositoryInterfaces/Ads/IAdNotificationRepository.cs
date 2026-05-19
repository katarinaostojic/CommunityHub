using CommunityHub.Application.Domain.Ads;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Ads;

public interface IAdNotificationRepository
{
    void Create(long recipientId, long adId, long bookedByAdId);
    List<AdNotification> GetUnreadByUser(long userId);
    void MarkAsRead(long notificationId);
    void MarkAllAsRead(long userId);
}
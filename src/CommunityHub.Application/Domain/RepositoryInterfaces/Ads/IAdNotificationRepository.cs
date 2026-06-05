using CommunityHub.Application.Domain.Entities.Ads;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Ads;

public interface IAdNotificationRepository
{
    void CreateBookingNotification(long recipientId, long adId, long bookedByAdId);
    void CreateMatchingAdNotification(long recipientId, long adId, long matchingAdId);
    List<AdNotification> GetByUser(long userId);
    List<AdNotification> GetUnreadByUser(long userId);
    void MarkAsRead(long notificationId);
    void MarkAllAsRead(long userId);
}
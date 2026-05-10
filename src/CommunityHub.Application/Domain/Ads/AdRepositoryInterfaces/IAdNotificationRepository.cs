namespace CommunityHub.Application.Domain.Ads.AdRepositoryInterfaces;

public interface IAdNotificationRepository
{
    void Create(long recipientId, long adId, long bookedByAdId);
    List<AdNotification> GetUnreadByUser(long userId);
    void MarkAllAsRead(long userId);
}
namespace CommunityHub.Application.Domain.Entities.Ads;

public class AdNotification
{
    public long Id { get; private set; }
    public long RecipientId { get; private set; }
    public Ad Ad { get; private set; }
    public Ad RelatedAd { get; private set; }
    public AdNotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

    public AdNotification(
        long id,
        long recipientId,
        Ad ad,
        Ad relatedAd,
        AdNotificationType type,
        DateTime createdAt,
        bool isRead)
    {
        Id = id;
        RecipientId = recipientId;
        Ad = ad;
        RelatedAd = relatedAd;
        Type = type;
        CreatedAt = createdAt;
        IsRead = isRead;
    }
}
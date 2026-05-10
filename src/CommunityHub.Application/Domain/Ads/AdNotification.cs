namespace CommunityHub.Application.Domain.Ads;

public class AdNotification
{
    public long Id { get; private set; }
    public long RecipientId { get; private set; }
    public Ad Ad { get; private set; }
    public Ad BookedByAd { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

    public AdNotification(long id, long recipientId, Ad ad, Ad bookedByAd,
        DateTime createdAt, bool isRead)
    {
        Id = id;
        RecipientId = recipientId;
        Ad = ad;
        BookedByAd = bookedByAd;
        CreatedAt = createdAt;
        IsRead = isRead;
    }

    public void MarkAsRead() => IsRead = true;
}
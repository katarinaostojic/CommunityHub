namespace CommunityHub.Application.DTOs.Ads;

public class AdNotificationDto
{
    public AdNotificationDto(
        long id,
        long recipientId,
        AdDto ad,
        AdDto bookedByAd,
        DateTime createdAt,
        bool isRead)
    {
        Id = id;
        RecipientId = recipientId;
        Ad = ad;
        BookedByAd = bookedByAd;
        CreatedAt = createdAt;
        IsRead = isRead;
    }

    public long Id { get; init; }
    public long RecipientId { get; init; }
    public AdDto Ad { get; init; }
    public AdDto BookedByAd { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
}
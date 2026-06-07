using CommunityHub.Application.Domain.Entities.Buildings.Ads;

namespace CommunityHub.Application.DTOs.Buildings.Ads;

public class AdNotificationDto
{
    public AdNotificationDto(
        long id,
        long recipientId,
        AdDto ad,
        AdDto relatedAd,
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

    public long Id { get; init; }
    public long RecipientId { get; init; }
    public AdDto Ad { get; init; }
    public AdDto RelatedAd { get; init; }
    public AdNotificationType Type { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
}
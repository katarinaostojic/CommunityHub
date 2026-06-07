using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings.Ads;

public static class AdNotificationMapper
{
    private static readonly UserColumnAliases RelatedAuthorAliases = new(
        "ra_user_id",
        "ra_username",
        "ra_password",
        "ra_name",
        "ra_surname",
        "ra_birthday",
        "ra_role");

    public static AdNotification Map(IDataReader reader)
    {
        User adAuthor = UserMapper.Map(reader);
        Ad ad = AdMapper.Map(reader, adAuthor);

        User relatedAuthor = UserMapper.MapWithAliases(reader, RelatedAuthorAliases);
        Ad relatedAd = AdMapper.MapRelatedAd(reader, relatedAuthor);

        return new AdNotification(
            id: Convert.ToInt64(reader["notif_id"]),
            recipientId: Convert.ToInt64(reader["recipient_id"]),
            ad: ad,
            relatedAd: relatedAd,
            type: AdMapper.MapNotificationType(reader["notification_type"].ToString()!),
            createdAt: Convert.ToDateTime(reader["created_at"]),
            isRead: Convert.ToBoolean(reader["is_read"]));
    }
}
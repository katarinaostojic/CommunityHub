using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Ads;

public static class AdNotificationMapper
{
    private static readonly UserColumnAliases BookedByAuthorAliases = new(
        "ub_id",
        "ub_username",
        "ub_password",
        "ub_name",
        "ub_surname",
        "ub_birthday",
        "ub_role");

    public static AdNotification Map(IDataReader reader)
    {
        User adAuthor = UserMapper.Map(reader);
        Ad ad = AdMapper.Map(reader, adAuthor);

        User bookedByAuthor = UserMapper.MapWithAliases(reader, BookedByAuthorAliases);
        Ad bookedByAd = AdMapper.MapBookedByAd(reader, bookedByAuthor);

        return new AdNotification(
            id: Convert.ToInt64(reader["notif_id"]),
            recipientId: Convert.ToInt64(reader["recipient_id"]),
            ad: ad,
            bookedByAd: bookedByAd,
            createdAt: Convert.ToDateTime(reader["created_at"]),
            isRead: Convert.ToBoolean(reader["is_read"]));
    }
}
using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Mappers.Ads;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Ads.AdRepositoryInterfaces;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Ads;

public class AdNotificationDbRepository : BaseDbRepository, IAdNotificationRepository
{
    public void Create(long recipientId, long adId, long bookedByAdId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO notice_board_notifications (recipient_id, ad_id, booked_by_ad_id)
            VALUES (@recipientId, @adId, @bookedByAdId)";

        AddParameter(command, "@recipientId", recipientId);
        AddParameter(command, "@adId", adId);
        AddParameter(command, "@bookedByAdId", bookedByAdId);
        command.ExecuteNonQuery();
    }

    public List<AdNotification> GetUnreadByUser(long userId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT n.id AS notif_id, n.recipient_id, n.created_at, n.is_read,
                   a.id, a.building_id, a.type, a.category, a.description,
                   a.date_from, a.date_to, a.status,
                   ua.id AS user_id, ua.username, ua.password,
                   ua.name, ua.surname, ua.birthday, ua.role,
                   b.id AS ba_id, b.building_id AS ba_building_id,
                   b.type AS ba_type, b.category AS ba_category,
                   b.description AS ba_description,
                   b.date_from AS ba_date_from, b.date_to AS ba_date_to,
                   b.status AS ba_status,
                   ub.id AS ub_id, ub.username AS ub_username, ub.password AS ub_password,
                   ub.name AS ub_name, ub.surname AS ub_surname,
                   ub.birthday AS ub_birthday, ub.role AS ub_role
            FROM notice_board_notifications n
            JOIN notice_board_ads a ON a.id = n.ad_id
            JOIN users ua ON ua.id = a.user_id
            JOIN notice_board_ads b ON b.id = n.booked_by_ad_id
            JOIN users ub ON ub.id = b.user_id
            WHERE n.recipient_id = @userId AND n.is_read = FALSE
            ORDER BY n.created_at DESC";

        AddParameter(command, "@userId", userId);

        using IDataReader reader = command.ExecuteReader();
        List<AdNotification> notifications = new();
        while (reader.Read())
        {
            User adAuthor = UserMapper.Map(reader);
            Ad ad = AdMapper.Map(reader, adAuthor);

            User bookedByAuthor = UserMapper.MapWithAliases(reader,
                "ub_id", "ub_username", "ub_password",
                "ub_name", "ub_surname", "ub_birthday", "ub_role");
            Ad bookedByAd = AdMapper.MapBookedByAd(reader, bookedByAuthor);

            notifications.Add(new AdNotification(
                id: Convert.ToInt64(reader["notif_id"]),
                recipientId: Convert.ToInt64(reader["recipient_id"]),
                ad: ad,
                bookedByAd: bookedByAd,
                createdAt: Convert.ToDateTime(reader["created_at"]),
                isRead: Convert.ToBoolean(reader["is_read"])
            ));
        }
        return notifications;
    }

    public void MarkAllAsRead(long userId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE notice_board_notifications
            SET is_read = TRUE
            WHERE recipient_id = @userId";

        AddParameter(command, "@userId", userId);
        command.ExecuteNonQuery();
    }
}
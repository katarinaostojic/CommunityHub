using CommunityHub.Application.Database.Mappers.Ads;
using CommunityHub.Application.Database.Readers.Ads;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Ads;

public class AdNotificationDbRepository : BaseDbRepository, IAdNotificationRepository
{
    public void CreateBookingNotification(long recipientId, long adId, long bookedByAdId)
    {
        Create(recipientId, adId, bookedByAdId, AdNotificationType.Booking);
    }

    public void CreateMatchingAdNotification(long recipientId, long adId, long matchingAdId)
    {
        Create(recipientId, adId, matchingAdId, AdNotificationType.MatchingAd);
    }

    public List<AdNotification> GetByUser(long userId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
            {SelectNotificationsSql()}
            WHERE n.recipient_id = @userId
            ORDER BY n.created_at DESC";

        AddParameter(command, "@userId", userId);

        using IDataReader reader = command.ExecuteReader();
        return AdNotificationReader.ReadNotifications(reader);
    }

    public List<AdNotification> GetUnreadByUser(long userId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = $@"
            {SelectNotificationsSql()}
            WHERE n.recipient_id = @userId
              AND n.is_read = FALSE
            ORDER BY n.created_at DESC";

        AddParameter(command, "@userId", userId);

        using IDataReader reader = command.ExecuteReader();
        return AdNotificationReader.ReadNotifications(reader);
    }

    public void MarkAsRead(long notificationId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE notice_board_notifications
            SET is_read = TRUE
            WHERE id = @notificationId";

        AddParameter(command, "@notificationId", notificationId);

        command.ExecuteNonQuery();
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

    private void Create(long recipientId, long adId, long relatedAdId, AdNotificationType type)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO notice_board_notifications (recipient_id, ad_id, related_ad_id, type)
            VALUES (@recipientId, @adId, @relatedAdId, @type::ad_notification_type)";

        AddParameter(command, "@recipientId", recipientId);
        AddParameter(command, "@adId", adId);
        AddParameter(command, "@relatedAdId", relatedAdId);
        AddParameter(command, "@type", AdMapper.ToDbNotificationType(type));

        command.ExecuteNonQuery();
    }

    private static string SelectNotificationsSql()
    {
        return @"
            SELECT n.id AS notif_id,
                   n.recipient_id,
                   n.created_at,
                   n.is_read,
                   n.type AS notification_type,
                   a.id AS id,
                   a.building_id AS building_id,
                   a.type AS type,
                   a.category AS category,
                   a.description AS description,
                   a.date_from AS date_from,
                   a.date_to AS date_to,
                   a.status AS status,
                   ua.id AS user_id,
                   ua.username,
                   ua.password,
                   ua.name,
                   ua.surname,
                   ua.birthday,
                   ua.role,
                   ra.id AS ra_id,
                   ra.building_id AS ra_building_id,
                   ra.type AS ra_type,
                   ra.category AS ra_category,
                   ra.description AS ra_description,
                   ra.date_from AS ra_date_from,
                   ra.date_to AS ra_date_to,
                   ra.status AS ra_status,
                   ura.id AS ra_user_id,
                   ura.username AS ra_username,
                   ura.password AS ra_password,
                   ura.name AS ra_name,
                   ura.surname AS ra_surname,
                   ura.birthday AS ra_birthday,
                   ura.role AS ra_role
            FROM notice_board_notifications n
            JOIN notice_board_ads a ON a.id = n.ad_id
            JOIN users ua ON ua.id = a.user_id
            JOIN notice_board_ads ra ON ra.id = n.related_ad_id
            JOIN users ura ON ura.id = ra.user_id";
    }
}
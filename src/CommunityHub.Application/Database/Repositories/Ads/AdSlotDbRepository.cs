using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Mappers.Ads;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Ads;

public class AdSlotDbRepository : BaseDbRepository, IAdSlotRepository
{
    public void CreateSlots(long adId, IEnumerable<(DateOnly date, TimeOnly start, TimeOnly end)> slots)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        foreach ((DateOnly date, TimeOnly start, TimeOnly end) in slots)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO notice_board_time_slots (ad_id, date, start_time, end_time)
                VALUES (@adId, @date, @start, @end)";

            AddParameter(command, "@adId", adId);
            AddParameter(command, "@date", ToUtcDateTime(date));
            AddParameter(command, "@start", start.ToTimeSpan());
            AddParameter(command, "@end", end.ToTimeSpan());
            command.ExecuteNonQuery();
        }
    }

    public List<AdSlot> GetSlotsByAd(long adId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time, b.booked_by_ad_id
            FROM notice_board_time_slots s
            LEFT JOIN notice_board_bookings b ON b.time_slot_id = s.id
            WHERE s.ad_id = @adId
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);

        using IDataReader reader = command.ExecuteReader();
        return AdSlotMapper.ReadSlots(reader);
    }

    public List<AdSlot> GetFreeSlotsByAd(long adId, DateOnly overlapFrom, DateOnly overlapTo)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time, b.booked_by_ad_id
            FROM notice_board_time_slots s
            LEFT JOIN notice_board_bookings b ON b.time_slot_id = s.id
            WHERE s.ad_id = @adId AND b.id IS NULL
              AND s.date >= @overlapFrom AND s.date <= @overlapTo
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);
        AddParameter(command, "@overlapFrom", ToUtcDateTime(overlapFrom));
        AddParameter(command, "@overlapTo", ToUtcDateTime(overlapTo));

        using IDataReader reader = command.ExecuteReader();
        return AdSlotMapper.ReadSlots(reader);
    }

    public void BookSlot(long slotId, long bookedByAdId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO notice_board_bookings (time_slot_id, booked_by_ad_id)
            VALUES (@slotId, @bookedByAdId)";

        AddParameter(command, "@slotId", slotId);
        AddParameter(command, "@bookedByAdId", bookedByAdId);
        command.ExecuteNonQuery();
    }

    public List<AdSlot> GetBookedSlotsByAd(long adId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time, b.booked_by_ad_id
            FROM notice_board_time_slots s
            JOIN notice_board_bookings b ON b.time_slot_id = s.id
            WHERE s.ad_id = @adId
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);

        using IDataReader reader = command.ExecuteReader();
        return AdSlotMapper.ReadSlots(reader);
    }

    public List<(AdSlot slot, Ad? bookedByAd)> GetBookedSlotsWithAds(long adId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time, b.booked_by_ad_id,
                   a.id AS ba_id, a.building_id AS ba_building_id, a.type AS ba_type,
                   a.category AS ba_category, a.description AS ba_description,
                   a.date_from AS ba_date_from, a.date_to AS ba_date_to, a.status AS ba_status,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM notice_board_time_slots s
            JOIN notice_board_bookings b ON b.time_slot_id = s.id
            LEFT JOIN notice_board_ads a ON a.id = b.booked_by_ad_id
            LEFT JOIN users u ON u.id = a.user_id
            WHERE s.ad_id = @adId
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);

        using IDataReader reader = command.ExecuteReader();
        return AdSlotMapper.ReadBookedSlotsWithAds(reader);
    }

    public User? GetTopHelperByBuilding(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role,
                   COUNT(b.id) AS booking_count
            FROM notice_board_bookings b
            JOIN notice_board_time_slots s ON b.time_slot_id = s.id
            JOIN notice_board_ads a ON s.ad_id = a.id
            JOIN users u ON a.user_id = u.id
            WHERE a.building_id = @buildingId AND a.type = 'offering'
            GROUP BY u.id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            ORDER BY booking_count DESC
            LIMIT 1";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read() ? UserMapper.Map(reader) : null;
    }

    private static DateTime ToUtcDateTime(DateOnly date)
    {
        return DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
    }
}
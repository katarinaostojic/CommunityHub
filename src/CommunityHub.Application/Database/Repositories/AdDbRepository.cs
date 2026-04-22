using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class AdDbRepository : BaseDbRepository, IAdRepository
{
    public List<Ad> GetActiveByBuilding(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT a.id, a.building_id, a.type, a.category, a.description,
                   a.date_from, a.date_to, a.status,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM notice_board_ads a
            JOIN users u ON a.user_id = u.id
            WHERE a.building_id = @buildingId
              AND a.status = 'active'
            ORDER BY a.id DESC";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return ReadAds(reader);
    }

    public Ad? GetById(long adId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT a.id, a.building_id, a.type, a.category, a.description,
                   a.date_from, a.date_to, a.status,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM notice_board_ads a
            JOIN users u ON a.user_id = u.id
            WHERE a.id = @adId";

        AddParameter(command, "@adId", adId);

        using IDataReader reader = command.ExecuteReader();
        List<Ad> ads = ReadAds(reader);
        return ads.Count == 0 ? null : ads[0];
    }

    public long Create(long buildingId, long authorId, AdType type, AdCategory category,
        string description, DateOnly dateFrom, DateOnly dateTo)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO notice_board_ads
                (building_id, user_id, type, category, description, date_from, date_to)
            VALUES
                (@buildingId, @authorId, @type::ad_type, @category::ad_category,
                 @description, @dateFrom, @dateTo)
            RETURNING id";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@authorId", authorId);
        AddParameter(command, "@type", AdMapper.ToDbType(type));
        AddParameter(command, "@category", AdMapper.ToDbCategory(category));
        AddParameter(command, "@description", description);
        AddParameter(command, "@dateFrom", DateTime.SpecifyKind(dateFrom.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));
        AddParameter(command, "@dateTo", DateTime.SpecifyKind(dateTo.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void Archive(long adId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE notice_board_ads
            SET status = 'archived'
            WHERE id = @adId";

        AddParameter(command, "@adId", adId);
        command.ExecuteNonQuery();
    }

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
            AddParameter(command, "@date", DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));
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
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time,
                   b.booked_by_ad_id
            FROM notice_board_time_slots s
            LEFT JOIN notice_board_bookings b ON b.time_slot_id = s.id
            WHERE s.ad_id = @adId
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);

        using IDataReader reader = command.ExecuteReader();
        return ReadSlots(reader);
    }

    public List<AdSlot> GetFreeSlotsByAd(long adId, DateOnly overlapFrom, DateOnly overlapTo)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time,
                   b.booked_by_ad_id
            FROM notice_board_time_slots s
            LEFT JOIN notice_board_bookings b ON b.time_slot_id = s.id
            WHERE s.ad_id = @adId
              AND b.id IS NULL
              AND s.date >= @overlapFrom
              AND s.date <= @overlapTo
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);
        AddParameter(command, "@overlapFrom", DateTime.SpecifyKind(overlapFrom.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));
        AddParameter(command, "@overlapTo", DateTime.SpecifyKind(overlapTo.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));

        using IDataReader reader = command.ExecuteReader();
        return ReadSlots(reader);
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
            SELECT s.id AS slot_id, s.ad_id, s.date, s.start_time, s.end_time,
                   b.booked_by_ad_id
            FROM notice_board_time_slots s
            JOIN notice_board_bookings b ON b.time_slot_id = s.id
            WHERE s.ad_id = @adId
            ORDER BY s.date, s.start_time";

        AddParameter(command, "@adId", adId);

        using IDataReader reader = command.ExecuteReader();
        return ReadSlots(reader);
    }

    private List<Ad> ReadAds(IDataReader reader)
    {
        List<Ad> ads = new List<Ad>();
        while (reader.Read())
        {
            User author = UserMapper.Map(reader);
            ads.Add(AdMapper.Map(reader, author));
        }
        return ads;
    }

    private List<AdSlot> ReadSlots(IDataReader reader)
    {
        List<AdSlot> slots = new List<AdSlot>();
        while (reader.Read())
            slots.Add(AdMapper.MapSlot(reader));
        return slots;
    }
}
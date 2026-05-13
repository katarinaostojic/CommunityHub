using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Mappers.Ads;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Ads;

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

    public void Update(Ad ad)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE notice_board_ads
            SET status = @status::ad_status
            WHERE id = @id";

        AddParameter(command, "@id", ad.Id);
        AddParameter(command, "@status", AdMapper.ToDbStatus(ad.Status));
        command.ExecuteNonQuery();
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

    public List<Ad> GetAllByBuilding(long buildingId)
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
        ORDER BY a.id DESC";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return ReadAds(reader);
    }
}
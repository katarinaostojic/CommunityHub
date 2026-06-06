using CommunityHub.Application.Database.Mappers.Buildings.Ads;
using CommunityHub.Application.Database.Readers.Buildings.Ads;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.Ads;

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
        return AdReader.ReadAds(reader);
    }

    public List<Ad> GetFilteredActiveByBuilding(long buildingId, AdType? type, AdCategory? category)
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
          AND (@type IS NULL OR a.type = @type::ad_type)
          AND (@category IS NULL OR a.category = @category::ad_category)
        ORDER BY a.id DESC";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@type", GetTypeParameterValue(type));
        AddParameter(command, "@category", GetCategoryParameterValue(category));

        using IDataReader reader = command.ExecuteReader();
        return AdReader.ReadAds(reader);
    }

    public int CountFilteredActiveByBuilding(long buildingId, AdType? type, AdCategory? category)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT COUNT(*)
        FROM notice_board_ads
        WHERE building_id = @buildingId
          AND status = 'active'
          AND (@type IS NULL OR type = @type::ad_type)
          AND (@category IS NULL OR category = @category::ad_category)";

        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@type", GetTypeParameterValue(type));
        AddParameter(command, "@category", GetCategoryParameterValue(category));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static string? GetTypeParameterValue(AdType? type)
    {
        return type.HasValue
            ? AdMapper.ToDbType(type.Value)
            : null;
    }

    private static string? GetCategoryParameterValue(AdCategory? category)
    {
        return category.HasValue
            ? AdMapper.ToDbCategory(category.Value)
            : null;
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
        return AdReader.ReadSingleAd(reader);
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
        return AdReader.ReadAds(reader);
    }

    public long Create(Ad ad)
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

        AddParameter(command, "@buildingId", ad.BuildingId);
        AddParameter(command, "@authorId", ad.Author.Id);
        AddParameter(command, "@type", AdMapper.ToDbType(ad.Type));
        AddParameter(command, "@category", AdMapper.ToDbCategory(ad.Category));
        AddParameter(command, "@description", ad.Description);
        AddParameter(command, "@dateFrom", ToUtcDateTime(ad.DateFrom));
        AddParameter(command, "@dateTo", ToUtcDateTime(ad.DateTo));

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public bool HasActiveMatchBefore(Ad ad, long beforeAdId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT EXISTS (
            SELECT 1
            FROM notice_board_ads candidate
            WHERE candidate.id < @beforeAdId
              AND candidate.building_id = @buildingId
              AND candidate.user_id <> @authorId
              AND candidate.status = 'active'
              AND candidate.type = @type::ad_type
              AND candidate.category = @category::ad_category
              AND candidate.date_from <= @dateTo
              AND candidate.date_to >= @dateFrom
        )";

        AddParameter(command, "@beforeAdId", beforeAdId);
        AddParameter(command, "@buildingId", ad.BuildingId);
        AddParameter(command, "@authorId", ad.Author.Id);
        AddParameter(command, "@type", AdMapper.ToDbType(ad.OppositeType));
        AddParameter(command, "@category", AdMapper.ToDbCategory(ad.Category));
        AddParameter(command, "@dateFrom", ToUtcDateTime(ad.DateFrom));
        AddParameter(command, "@dateTo", ToUtcDateTime(ad.DateTo));

        return Convert.ToBoolean(command.ExecuteScalar());
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

    private static DateTime ToUtcDateTime(DateOnly date)
    {
        return DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
    }
}
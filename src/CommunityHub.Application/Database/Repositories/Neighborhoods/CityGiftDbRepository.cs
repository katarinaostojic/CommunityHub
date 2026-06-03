using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class CityGiftDbRepository : BaseDbRepository, ICityGiftRepository
{
    public List<CityGift> GetAll()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, amount, deadline, awarded_neighborhood_id, is_awarded, created_at
            FROM city_gifts
            ORDER BY deadline DESC";

        using IDataReader reader = command.ExecuteReader();
        List<CityGift> gifts = new();
        while (reader.Read())
            gifts.Add(MapGift(reader));
        return gifts;
    }

    public CityGift? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, amount, deadline, awarded_neighborhood_id, is_awarded, created_at
            FROM city_gifts WHERE id = @id";
        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return MapGift(reader);
    }

    public List<CityGiftApplication> GetApplications(long cityGiftId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, city_gift_id, coordinator_id, neighborhood_id, applied_at
            FROM city_gift_applications
            WHERE city_gift_id = @cityGiftId";
        AddParameter(command, "@cityGiftId", cityGiftId);

        using IDataReader reader = command.ExecuteReader();
        List<CityGiftApplication> apps = new();
        while (reader.Read())
            apps.Add(new CityGiftApplication(
                Convert.ToInt64(reader["id"]),
                Convert.ToInt64(reader["city_gift_id"]),
                Convert.ToInt64(reader["coordinator_id"]),
                Convert.ToInt64(reader["neighborhood_id"]),
                (DateOnly)reader["applied_at"]));
        return apps;
    }

    public void Apply(long cityGiftId, long coordinatorId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO city_gift_applications (city_gift_id, coordinator_id, neighborhood_id, applied_at)
            VALUES (@cityGiftId, @coordinatorId, @neighborhoodId, @appliedAt)";
        AddParameter(command, "@cityGiftId", cityGiftId);
        AddParameter(command, "@coordinatorId", coordinatorId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@appliedAt", DateTime.UtcNow);
        command.ExecuteNonQuery();
    }

    public void Award(long cityGiftId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE city_gifts
            SET is_awarded = true, awarded_neighborhood_id = @neighborhoodId
            WHERE id = @cityGiftId";
        AddParameter(command, "@cityGiftId", cityGiftId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        command.ExecuteNonQuery();
    }

    public void AddDonationToCategories(long neighborhoodId, decimal amountPerCategory)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE neighborhoods SET budget = budget + @total WHERE id = @neighborhoodId;

            INSERT INTO donations (citizen_id, neighborhood_id, category_id, amount, created_at)
            SELECT
                (SELECT id FROM users WHERE role = 'admin' LIMIT 1),
                @neighborhoodId,
                dc.id,
                @amountPerCategory,
                CURRENT_DATE
            FROM donation_categories dc";
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@amountPerCategory", amountPerCategory);
        AddParameter(command, "@total", amountPerCategory * 6);
        command.ExecuteNonQuery();
    }

    public List<(long NeighborhoodId, decimal Budget)> GetBudgetsForNeighborhoods(List<long> neighborhoodIds)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, budget FROM neighborhoods
            WHERE id = ANY(@ids)";

        var param = command.CreateParameter();
        param.ParameterName = "@ids";
        param.Value = neighborhoodIds.ToArray();
        command.Parameters.Add(param);

        using IDataReader reader = command.ExecuteReader();
        List<(long, decimal)> result = new();
        while (reader.Read())
            result.Add((Convert.ToInt64(reader["id"]), Convert.ToDecimal(reader["budget"])));
        return result;
    }

    private CityGift MapGift(IDataReader reader)
    {
        long? awardedId = reader.IsDBNull(reader.GetOrdinal("awarded_neighborhood_id"))
            ? null : Convert.ToInt64(reader["awarded_neighborhood_id"]);

        return new CityGift(
            Convert.ToInt64(reader["id"]),
            Convert.ToDecimal(reader["amount"]),
            (DateOnly)reader["deadline"],
            awardedId,
            Convert.ToBoolean(reader["is_awarded"]),
            (DateOnly)reader["created_at"]);
    }
}
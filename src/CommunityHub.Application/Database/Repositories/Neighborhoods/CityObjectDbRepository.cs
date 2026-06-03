using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class CityObjectDbRepository : BaseDbRepository, ICityObjectRepository
{
    public List<CityObject> GetAll()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT co.id, co.name, co.description,
                   COUNT(cov.id) AS vote_count
            FROM city_objects co
            LEFT JOIN city_object_votes cov ON cov.city_object_id = co.id
            GROUP BY co.id, co.name, co.description
            ORDER BY vote_count DESC, co.name ASC";

        using IDataReader reader = command.ExecuteReader();
        List<CityObject> objects = new();
        while (reader.Read())
            objects.Add(MapCityObject(reader));

        return objects;
    }

    public CityObject? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT co.id, co.name, co.description,
                   COUNT(cov.id) AS vote_count
            FROM city_objects co
            LEFT JOIN city_object_votes cov ON cov.city_object_id = co.id
            WHERE co.id = @id
            GROUP BY co.id, co.name, co.description";

        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return MapCityObject(reader);
    }

    public List<CityObjectReservation> GetReservations(long cityObjectId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, city_object_id, neighborhood_id, date_from, date_to
            FROM city_object_reservations
            WHERE city_object_id = @cityObjectId";

        AddParameter(command, "@cityObjectId", cityObjectId);

        using IDataReader reader = command.ExecuteReader();
        List<CityObjectReservation> reservations = new();
        while (reader.Read())
            reservations.Add(MapReservation(reader));

        return reservations;
    }

    public void AddReservation(CityObjectReservation reservation)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO city_object_reservations (city_object_id, neighborhood_id, date_from, date_to)
            VALUES (@cityObjectId, @neighborhoodId, @dateFrom, @dateTo)";

        AddParameter(command, "@cityObjectId", reservation.CityObjectId);
        AddParameter(command, "@neighborhoodId", reservation.NeighborhoodId);
        AddParameter(command, "@dateFrom",
            DateTime.SpecifyKind(reservation.DateFrom.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));
        AddParameter(command, "@dateTo",
            DateTime.SpecifyKind(reservation.DateTo.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc));

        command.ExecuteNonQuery();
    }

    public void ResetVotes(long cityObjectId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM city_object_votes
            WHERE city_object_id = @cityObjectId
              AND neighborhood_id = @neighborhoodId";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);

        command.ExecuteNonQuery();
    }

    private CityObject MapCityObject(IDataReader reader)
    {
        return new CityObject(
            Convert.ToInt64(reader["id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            Convert.ToInt32(reader["vote_count"])
        );
    }

    private CityObjectReservation MapReservation(IDataReader reader)
    {
        return new CityObjectReservation(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["city_object_id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            (DateOnly)reader["date_from"],
            (DateOnly)reader["date_to"]
        );
    }
}

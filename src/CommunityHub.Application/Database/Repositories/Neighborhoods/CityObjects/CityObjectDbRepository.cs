using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods.CityObjects;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.CityObjects;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods.CityObjects;

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
    public List<CityObject> GetByNeighborhood(long neighborhoodId, long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT co.id, co.name, co.description,
               COUNT(DISTINCT cov.id) AS vote_count,
               MAX(cor.date_to) AS last_visit,
               CASE WHEN EXISTS (
                   SELECT 1 FROM city_object_votes
                   WHERE city_object_id = co.id AND citizen_id = @citizenId
               ) THEN true ELSE false END AS has_voted
        FROM city_objects co
        LEFT JOIN city_object_votes cov ON cov.city_object_id = co.id 
            AND cov.neighborhood_id = @neighborhoodId
        LEFT JOIN city_object_reservations cor ON cor.city_object_id = co.id 
            AND cor.neighborhood_id = @neighborhoodId
        GROUP BY co.id, co.name, co.description
        ORDER BY vote_count DESC, co.name ASC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@citizenId", citizenId);

        using IDataReader reader = command.ExecuteReader();
        List<CityObject> objects = new();
        while (reader.Read())
            objects.Add(MapCityObjectFull(reader));
        return objects;
    }

    public bool HasVoted(long cityObjectId, long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM city_object_votes
        WHERE city_object_id = @cityObjectId AND citizen_id = @citizenId";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@citizenId", citizenId);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public void AddVote(long cityObjectId, long citizenId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO city_object_votes (city_object_id, citizen_id, neighborhood_id)
        VALUES (@cityObjectId, @citizenId, @neighborhoodId)";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        command.ExecuteNonQuery();
    }

    public void RemoveVote(long cityObjectId, long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        DELETE FROM city_object_votes
        WHERE city_object_id = @cityObjectId AND citizen_id = @citizenId";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@citizenId", citizenId);
        command.ExecuteNonQuery();
    }

    public int GetVoteCount(long cityObjectId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT COUNT(*) FROM city_object_votes
        WHERE city_object_id = @cityObjectId AND neighborhood_id = @neighborhoodId";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public DateOnly? GetLastVisit(long cityObjectId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT MAX(date_to) FROM city_object_reservations
        WHERE city_object_id = @cityObjectId AND neighborhood_id = @neighborhoodId";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        object? result = command.ExecuteScalar();
        if (result == null || result == DBNull.Value) return null;
        return (DateOnly)result;
    }

    public List<CityObjectReservation> GetReservationsByNeighborhood(long neighborhoodId, int? month, int? year)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, city_object_id, neighborhood_id, date_from, date_to
        FROM city_object_reservations
        WHERE neighborhood_id = @neighborhoodId
          AND (@year IS NULL OR EXTRACT(YEAR FROM date_from) = @year)
          AND (@month IS NULL OR EXTRACT(MONTH FROM date_from) = @month)
        ORDER BY date_from DESC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@year", year);
        AddParameter(command, "@month", month);

        using IDataReader reader = command.ExecuteReader();
        List<CityObjectReservation> reservations = new();
        while (reader.Read())
            reservations.Add(MapReservation(reader));
        return reservations;
    }

    public List<CityObjectReservation> GetReservationsByCityObject(long cityObjectId, long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, city_object_id, neighborhood_id, date_from, date_to
        FROM city_object_reservations
        WHERE city_object_id = @cityObjectId AND neighborhood_id = @neighborhoodId
        ORDER BY date_from DESC";

        AddParameter(command, "@cityObjectId", cityObjectId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        List<CityObjectReservation> reservations = new();
        while (reader.Read())
            reservations.Add(MapReservation(reader));
        return reservations;
    }

    private CityObject MapCityObjectFull(IDataReader reader)
    {
        DateOnly? lastVisit = reader.IsDBNull(reader.GetOrdinal("last_visit"))
            ? null : (DateOnly)reader["last_visit"];
        bool hasVoted = Convert.ToBoolean(reader["has_voted"]);

        var obj = new CityObject(
            Convert.ToInt64(reader["id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            Convert.ToInt32(reader["vote_count"]),
            lastVisit,
            hasVoted
        );
        return obj;
    }
    public List<CityObject> GetAllByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT co.id, co.name, co.description,
               COUNT(cov.id) AS vote_count
        FROM city_objects co
        LEFT JOIN city_object_votes cov ON cov.city_object_id = co.id
            AND cov.neighborhood_id = @neighborhoodId
        GROUP BY co.id, co.name, co.description
        ORDER BY vote_count DESC, co.name ASC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        List<CityObject> objects = new();
        while (reader.Read())
            objects.Add(MapCityObject(reader));

        return objects;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class CoordinatorReviewDbRepository : BaseDbRepository, ICoordinatorReviewRepository
{
    public void Create(long citizenId, long coordinatorId, long neighborhoodId, int rating, string? comment)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO coordinator_reviews 
                (citizen_id, coordinator_id, neighborhood_id, rating, comment, created_at, report_count, is_removed)
            VALUES 
                (@citizenId, @coordinatorId, @neighborhoodId, @rating, @comment, @createdAt, 0, false)";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@coordinatorId", coordinatorId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@rating", rating);
        AddParameter(command, "@comment", comment);
        AddParameter(command, "@createdAt", DateTime.UtcNow);
        command.ExecuteNonQuery();
    }

    public List<CoordinatorReview> GetByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT r.id, r.citizen_id, r.coordinator_id, r.neighborhood_id,
               r.rating, r.comment, r.created_at, r.report_count, r.is_removed,
               u.name AS citizen_name, u.surname AS citizen_surname
        FROM coordinator_reviews r
        JOIN users u ON r.citizen_id = u.id
        WHERE r.neighborhood_id = @neighborhoodId AND r.is_removed = false
        ORDER BY r.created_at DESC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        using IDataReader reader = command.ExecuteReader();
        return ReadReviews(reader);
    }

    public CoordinatorReview? GetById(long reviewId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT r.id, r.citizen_id, r.coordinator_id, r.neighborhood_id,
               r.rating, r.comment, r.created_at, r.report_count, r.is_removed,
               u.name AS citizen_name, u.surname AS citizen_surname
        FROM coordinator_reviews r
        JOIN users u ON r.citizen_id = u.id
        WHERE r.id = @id";

        AddParameter(command, "@id", reviewId);
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read()) return MapReview(reader);
        return null;
    }

    public bool HasReviewedThisMonth(long citizenId, long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM coordinator_reviews
            WHERE citizen_id = @citizenId
              AND coordinator_id = @coordinatorId
              AND DATE_TRUNC('month', created_at) = DATE_TRUNC('month', CURRENT_DATE)
              AND is_removed = false";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@coordinatorId", coordinatorId);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public int GetRemovedReviewCount(long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM coordinator_reviews
            WHERE citizen_id = @citizenId AND is_removed = true";

        AddParameter(command, "@citizenId", citizenId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void AddReport(long reviewId, long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO coordinator_review_reports (review_id, citizen_id, created_at)
            VALUES (@reviewId, @citizenId, @createdAt);

            UPDATE coordinator_reviews
            SET report_count = report_count + 1
            WHERE id = @reviewId";

        AddParameter(command, "@reviewId", reviewId);
        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@createdAt", DateTime.UtcNow);
        command.ExecuteNonQuery();
    }

    public bool HasAlreadyReported(long reviewId, long citizenId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) FROM coordinator_review_reports
            WHERE review_id = @reviewId AND citizen_id = @citizenId";

        AddParameter(command, "@reviewId", reviewId);
        AddParameter(command, "@citizenId", citizenId);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    public void Remove(long reviewId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE coordinator_reviews SET is_removed = true WHERE id = @id";

        AddParameter(command, "@id", reviewId);
        command.ExecuteNonQuery();
    }

    public List<long> GetReporterIds(long reviewId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT citizen_id FROM coordinator_review_reports
        WHERE review_id = @reviewId";

        AddParameter(command, "@reviewId", reviewId);
        using IDataReader reader = command.ExecuteReader();
        List<long> ids = new();
        while (reader.Read())
            ids.Add(Convert.ToInt64(reader["citizen_id"]));
        return ids;
    }

    private List<CoordinatorReview> ReadReviews(IDataReader reader)
    {
        List<CoordinatorReview> reviews = new();
        while (reader.Read())
            reviews.Add(MapReview(reader));
        return reviews;
    }

    private CoordinatorReview MapReview(IDataReader reader)
    {
        return new CoordinatorReview(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["citizen_id"]),
            Convert.ToInt64(reader["coordinator_id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            Convert.ToInt32(reader["rating"]),
            reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader["comment"].ToString(),
            (DateOnly)reader["created_at"],
            Convert.ToInt32(reader["report_count"]),
            Convert.ToBoolean(reader["is_removed"]),
            reader["citizen_name"].ToString()! + " " + reader["citizen_surname"].ToString()!
        );
    }
}
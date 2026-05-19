using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class ForumDbRepository : BaseDbRepository, IForumRepository
{
    public long Create(Forum forum)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO forums (title, description, coordinator_id, is_closed, created_at)
            VALUES (@title, @description, @coordinatorId, false, @createdAt)
            RETURNING id";

        AddParameter(command, "@title", forum.Title);
        AddParameter(command, "@description", forum.Description);
        AddParameter(command, "@coordinatorId", forum.CoordinatorId);
        AddParameter(command, "@createdAt", forum.CreatedAt);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public List<Forum> GetAll(long currentCoordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT f.id, f.title, f.description, f.coordinator_id, f.is_closed, f.created_at,
                   u.name AS coordinator_name, u.surname AS coordinator_surname
            FROM forums f
            JOIN users u ON f.coordinator_id = u.id
            WHERE f.is_closed = false OR f.coordinator_id = @coordinatorId
            ORDER BY f.created_at DESC";

        AddParameter(command, "@coordinatorId", currentCoordinatorId);

        using IDataReader reader = command.ExecuteReader();
        List<Forum> forums = new();
        while (reader.Read())
            forums.Add(MapForum(reader));

        return forums;
    }

    public Forum? GetById(long id, long currentCoordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT f.id, f.title, f.description, f.coordinator_id, f.is_closed, f.created_at,
                   u.name AS coordinator_name, u.surname AS coordinator_surname
            FROM forums f
            JOIN users u ON f.coordinator_id = u.id
            WHERE f.id = @id AND (f.is_closed = false OR f.coordinator_id = @coordinatorId)";

        AddParameter(command, "@id", id);
        AddParameter(command, "@coordinatorId", currentCoordinatorId);

        using IDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        Forum forum = MapForum(reader);
        reader.Close();

        LoadComments(forum, currentCoordinatorId);
        return forum;
    }

    public void Update(Forum forum)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE forums SET is_closed = @isClosed WHERE id = @id";
        AddParameter(command, "@isClosed", forum.IsClosed);
        AddParameter(command, "@id", forum.Id);
        command.ExecuteNonQuery();
    }

    public long CreateComment(ForumComment comment)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO forum_comments (forum_id, coordinator_id, text, created_at)
            VALUES (@forumId, @coordinatorId, @text, @createdAt)
            RETURNING id";

        AddParameter(command, "@forumId", comment.ForumId);
        AddParameter(command, "@coordinatorId", comment.CoordinatorId);
        AddParameter(command, "@text", comment.Text);
        AddParameter(command, "@createdAt", comment.CreatedAt);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void AddReaction(long commentId, long coordinatorId, ReactionType reaction)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO forum_comment_reactions (comment_id, coordinator_id, reaction)
            VALUES (@commentId, @coordinatorId, @reaction)";

        AddParameter(command, "@commentId", commentId);
        AddParameter(command, "@coordinatorId", coordinatorId);
        AddParameter(command, "@reaction", reaction == ReactionType.Like ? "like" : "dislike");

        command.ExecuteNonQuery();
    }

    public void UpdateReaction(long commentId, long coordinatorId, ReactionType reaction)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE forum_comment_reactions 
            SET reaction = @reaction
            WHERE comment_id = @commentId AND coordinator_id = @coordinatorId";

        AddParameter(command, "@commentId", commentId);
        AddParameter(command, "@coordinatorId", coordinatorId);
        AddParameter(command, "@reaction", reaction == ReactionType.Like ? "like" : "dislike");

        command.ExecuteNonQuery();
    }

    public void RemoveReaction(long commentId, long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM forum_comment_reactions
            WHERE comment_id = @commentId AND coordinator_id = @coordinatorId";

        AddParameter(command, "@commentId", commentId);
        AddParameter(command, "@coordinatorId", coordinatorId);

        command.ExecuteNonQuery();
    }

    public ReactionType? GetReaction(long commentId, long coordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT reaction FROM forum_comment_reactions
            WHERE comment_id = @commentId AND coordinator_id = @coordinatorId";

        AddParameter(command, "@commentId", commentId);
        AddParameter(command, "@coordinatorId", coordinatorId);

        object? result = command.ExecuteScalar();
        if (result == null || result == DBNull.Value) return null;

        return result.ToString() == "like" ? ReactionType.Like : ReactionType.Dislike;
    }

    private void LoadComments(Forum forum, long currentCoordinatorId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT fc.id, fc.forum_id, fc.coordinator_id, fc.text, fc.created_at,
                   u.name, u.surname,
                   n.name AS neighborhood_name,
                   COALESCE(likes.cnt, 0) AS like_count,
                   COALESCE(dislikes.cnt, 0) AS dislike_count,
                   r.reaction AS current_reaction
            FROM forum_comments fc
            JOIN users u ON fc.coordinator_id = u.id
            LEFT JOIN neighborhoods n ON n.id = (SELECT MIN(id) FROM neighborhoods WHERE coordinator_id = fc.coordinator_id)
            LEFT JOIN (
                SELECT comment_id, COUNT(*) AS cnt FROM forum_comment_reactions 
                WHERE reaction = 'like' GROUP BY comment_id
            ) likes ON likes.comment_id = fc.id
            LEFT JOIN (
                SELECT comment_id, COUNT(*) AS cnt FROM forum_comment_reactions 
                WHERE reaction = 'dislike' GROUP BY comment_id
            ) dislikes ON dislikes.comment_id = fc.id
            LEFT JOIN forum_comment_reactions r 
                ON r.comment_id = fc.id AND r.coordinator_id = @currentCoordinatorId
            WHERE fc.forum_id = @forumId
            ORDER BY fc.created_at ASC";

        AddParameter(command, "@forumId", forum.Id);
        AddParameter(command, "@currentCoordinatorId", currentCoordinatorId);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
            forum.AddComment(MapComment(reader));
    }

    private Forum MapForum(IDataReader reader)
    {
        return new Forum(
            Convert.ToInt64(reader["id"]),
            reader["title"].ToString()!,
            reader["description"].ToString()!,
            Convert.ToInt64(reader["coordinator_id"]),
            reader["coordinator_name"].ToString()!,
            reader["coordinator_surname"].ToString()!,
            Convert.ToBoolean(reader["is_closed"]),
            Convert.ToDateTime(reader["created_at"])
        );
    }

    private ForumComment MapComment(IDataReader reader)
    {
        string? reactionStr = reader.IsDBNull(reader.GetOrdinal("current_reaction"))
            ? null : reader["current_reaction"].ToString();

        ReactionType? currentReaction = reactionStr == null ? null
            : reactionStr == "like" ? ReactionType.Like : ReactionType.Dislike;

        string? neighborhoodName = reader.IsDBNull(reader.GetOrdinal("neighborhood_name"))
            ? null : reader["neighborhood_name"].ToString();

        return new ForumComment(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["forum_id"]),
            Convert.ToInt64(reader["coordinator_id"]),
            reader["name"].ToString()!,
            reader["surname"].ToString()!,
            neighborhoodName ?? "Unknown",
            reader["text"].ToString()!,
            Convert.ToDateTime(reader["created_at"]),
            Convert.ToInt32(reader["like_count"]),
            Convert.ToInt32(reader["dislike_count"]),
            currentReaction
        );
    }
}
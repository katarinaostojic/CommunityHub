using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class ImageDbRepository : BaseDbRepository
{
    public List<Image> GetByEntity(string entity, long entityId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, path
            FROM images
            WHERE entity = @entity AND entity_id = @entityId";

        AddParameter(command, "@entity", entity);
        AddParameter(command, "@entityId", entityId);

        using IDataReader reader = command.ExecuteReader();
        return ReadImages(reader);
    }

    public Dictionary<long, List<Image>> GetByEntities(string entity, IEnumerable<long> entityIds)
    {
        List<long> ids = entityIds.Distinct().ToList();
        Dictionary<long, List<Image>> result = ids.ToDictionary(id => id, _ => new List<Image>());

        if (ids.Count == 0) return result;

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = BuildEntitiesCommand(connection, entity, ids);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long entityId = Convert.ToInt64(reader["entity_id"]);
            result[entityId].Add(new Image(
                Convert.ToInt64(reader["id"]),
                reader["path"].ToString()!
            ));
        }

        return result;
    }

    private IDbCommand BuildEntitiesCommand(IDbConnection connection, string entity, List<long> ids)
    {
        IDbCommand command = connection.CreateCommand();

        List<string> paramNames = new List<string>();
        for (int i = 0; i < ids.Count; i++)
        {
            string paramName = $"@id{i}";
            paramNames.Add(paramName);
            AddParameter(command, paramName, ids[i]);
        }

        command.CommandText = $@"
        SELECT id, path, entity_id
        FROM images
        WHERE entity = @entity AND entity_id IN ({string.Join(", ", paramNames)})";

        AddParameter(command, "@entity", entity);
        return command;
    }

    private List<Image> ReadImages(IDataReader reader)
    {
        List<Image> images = new List<Image>();
        while (reader.Read())
        {
            images.Add(new Image(
                Convert.ToInt64(reader["id"]),
                reader["path"].ToString()!
            ));
        }
        return images;
    }

    public void SaveImage(string entity, long entityId, string path)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO images (entity, entity_id, path)
        VALUES (@entity, @entityId, @path)";

        AddParameter(command, "@entity", entity);
        AddParameter(command, "@entityId", entityId);
        AddParameter(command, "@path", path);

        command.ExecuteNonQuery();
    }
}
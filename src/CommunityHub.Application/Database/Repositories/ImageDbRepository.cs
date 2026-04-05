using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class ImageDbRepository
{
    public List<AppImage> GetByResource(string resource, long resourceId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, path
            FROM images
            WHERE resource = @resource AND resource_id = @resourceId";

        AddParameter(command, "@resource", resource);
        AddParameter(command, "@resourceId", resourceId);

        using IDataReader reader = command.ExecuteReader();
        return ReadImages(reader);
    }

    public Dictionary<long, List<AppImage>> GetByResources(string resource, IEnumerable<long> resourceIds)
    {
        List<long> ids = resourceIds.Distinct().ToList();
        Dictionary<long, List<AppImage>> result = ids.ToDictionary(id => id, _ => new List<AppImage>());

        if (ids.Count == 0) return result;

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        List<string> paramNames = new List<string>();
        for (int i = 0; i < ids.Count; i++)
        {
            string paramName = $"@id{i}";
            paramNames.Add(paramName);
            AddParameter(command, paramName, ids[i]);
        }

        command.CommandText = $@"
            SELECT id, path, resource_id
            FROM images
            WHERE resource = @resource AND resource_id IN ({string.Join(", ", paramNames)})";

        AddParameter(command, "@resource", resource);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long resourceId = Convert.ToInt64(reader["resource_id"]);
            AppImage image = new AppImage(
                Convert.ToInt64(reader["id"]),
                reader["path"].ToString()!
            );
            result[resourceId].Add(image);
        }

        return result;
    }

    private List<AppImage> ReadImages(IDataReader reader)
    {
        List<AppImage> images = new List<AppImage>();
        while (reader.Read())
        {
            images.Add(new AppImage(
                Convert.ToInt64(reader["id"]),
                reader["path"].ToString()!
            ));
        }
        return images;
    }

    private void AddParameter(IDbCommand command, string name, string value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        param.DbType = DbType.String;
        command.Parameters.Add(param);
    }

    private void AddParameter(IDbCommand command, string name, long value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        command.Parameters.Add(param);
    }
}

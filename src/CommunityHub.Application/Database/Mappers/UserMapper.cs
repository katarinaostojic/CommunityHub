using CommunityHub.Application.Domain;
using System.Data;

namespace CommunityHub.Application.Database.Mappers;

public static class UserMapper
{
    public static User Map(IDataReader reader)
    {
        return new User(
            Convert.ToInt64(reader["user_id"]),
            reader["username"].ToString()!,
            reader["password"].ToString()!,
            reader["name"].ToString()!,
            reader["surname"].ToString()!,
            DateTime.Parse(reader["birthday"].ToString()!),
            reader["role"].ToString()!
        );
    }
}
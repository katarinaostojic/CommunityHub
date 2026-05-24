using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Users;

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
            ParseRole(reader["role"].ToString()!)
        );
    }

    // Used when a query joins multiple users in the same row (e.g. ad author and booking author).
    // Standard Map() always reads fixed column names like "user_id", "username" etc.
    // This overload accepts column name aliases so both users can be mapped from the same reader.
    public static User MapWithAliases(IDataReader reader, UserColumnAliases aliases)
    {
        return new User(
            Convert.ToInt64(reader[aliases.Id]),
            reader[aliases.Username].ToString()!,
            reader[aliases.Password].ToString()!,
            reader[aliases.Name].ToString()!,
            reader[aliases.Surname].ToString()!,
            DateTime.Parse(reader[aliases.Birthday].ToString()!),
            ParseRole(reader[aliases.Role].ToString()!)
        );
    }

    public static UserRole ParseRole(string role) => role switch
    {
        "tenant" => UserRole.Tenant,
        "manager" => UserRole.Manager,
        "coordinator" => UserRole.Coordinator,
        "citizen" => UserRole.Citizen,
        _ => throw new ArgumentException($"Unknown role: '{role}'")
    };
}
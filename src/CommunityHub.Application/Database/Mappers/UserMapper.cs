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
            ParseRole(reader["role"].ToString()!)
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
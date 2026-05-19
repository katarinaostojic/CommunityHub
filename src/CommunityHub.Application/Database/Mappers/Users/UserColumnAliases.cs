namespace CommunityHub.Application.Database.Mappers.Users;

public class UserColumnAliases
{
    public UserColumnAliases(
        string id,
        string username,
        string password,
        string name,
        string surname,
        string birthday,
        string role)
    {
        Id = id;
        Username = username;
        Password = password;
        Name = name;
        Surname = surname;
        Birthday = birthday;
        Role = role;
    }

    public string Id { get; }
    public string Username { get; }
    public string Password { get; }
    public string Name { get; }
    public string Surname { get; }
    public string Birthday { get; }
    public string Role { get; }
}
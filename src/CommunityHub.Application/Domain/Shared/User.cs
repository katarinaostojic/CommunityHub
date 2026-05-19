namespace CommunityHub.Application.Domain.Shared;

public enum UserRole
{
    Tenant,
    Manager,
    Coordinator,
    Citizen
}
public class User
{
    public long Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public DateTime BirthDay { get; private set; }
    public UserRole Role { get; private set; }
    public string? Address { get; private set; }
    public List<Post>? Posts { get; private set; }

    public string DisplayName => char.ToUpper(Name[0]) + Name.Substring(1).ToLower();
    public string FullName => $"{Name} {Surname}";

    public User(long id, string username, string password, string name, string surname, DateTime birthDay, UserRole role, string? address = null)
    {
        Id = id;
        Username = username;
        Password = password;
        Name = name;
        Surname = surname;
        BirthDay = birthDay;
        Role = role;
        Address = address;
        Posts = null;
    }

    public void AddPost(Post post)
    {
        if (Posts == null)
        {
            Posts = new List<Post>();
        }

        Posts.Add(post);
        post.User = this;
    }
}
using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Shared;

public class UserDbRepository
{
    public User? GetByCredentials(string username, string password)
    {
        // using blok automatski zatvara konekciju na kraju bloka u kom je pozvan
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, username, password, name, surname, birthday, role FROM users WHERE username = @username AND password = @password";

        // Parametrizovani upiti sprečavaju SQL injection napade
        IDbDataParameter usernameParam = command.CreateParameter();
        usernameParam.ParameterName = "@username";
        usernameParam.Value = username;
        command.Parameters.Add(usernameParam);

        IDbDataParameter passwordParam = command.CreateParameter();
        passwordParam.ParameterName = "@password";
        passwordParam.Value = password;
        command.Parameters.Add(passwordParam);

        // ExecuteReader vraća IDataReader za čitanje više redova
        using IDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User(
                Convert.ToInt64(reader["id"]),
                reader["username"].ToString(),
                reader["password"].ToString(),
                reader["name"].ToString(),
                reader["surname"].ToString(),
                Convert.ToDateTime(reader["birthday"].ToString()),
                UserMapper.ParseRole(reader["role"].ToString()!)
            );
        }

        return null;
    }

    public User? GetWithPosts(long userId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        // LEFT JOIN vraća korisnika čak i ako nema objave
        // Rezultat: ako korisnik ima 3 objave, dobijamo 3 reda sa istim korisnikom
        command.CommandText = @"
            SELECT u.id, u.username, u.password, u.name, u.surname, u.birthday, u.role,
                   p.id AS post_id, p.title, p.content, p.created_at
            FROM users u
            LEFT JOIN posts p ON u.id = p.user_id
            WHERE u.id = @userId";

        IDbDataParameter userIdParam = command.CreateParameter();
        userIdParam.ParameterName = "@userId";
        userIdParam.Value = userId;
        command.Parameters.Add(userIdParam);

        // ExecuteReader vraća IDataReader za čitanje više redova
        using IDataReader reader = command.ExecuteReader();

        User? user = null;

        // RED predstavlja jedan horizontalni red u tabeli rezultata SQL upita
        // Primer: Korisnik "Ana" sa 2 objave vraća 2 REDA:
        //   Red 1: [Ana, Anić, 1998-08-22, Post1_id, "Naslov 1", "Sadržaj 1", 2024-01-16]
        //   Red 2: [Ana, Anić, 1998-08-22, Post2_id, "Naslov 2", "Sadržaj 2", 2024-01-19]
        // Reader je kao kursor koji ide red-po-red odozgo nadole
        // Svaki reader.Read() pomera kursor na sledeći red i vraća true dok god ima redova
        while (reader.Read())
        {
            // Kreiramo User objekat samo jednom (prvi red)
            if (user == null)
            {
                user = new User(
                    Convert.ToInt64(reader["id"]),
                    reader["username"].ToString(),
                    reader["password"].ToString(),
                    reader["name"].ToString(),
                    reader["surname"].ToString(),
                    Convert.ToDateTime(reader["birthday"].ToString()),
                    UserMapper.ParseRole(reader["role"].ToString()!)
                );
            }

            // IsDBNull proverava da li je vrednost NULL u bazi
            // Ako korisnik nema objave, post_id će biti NULL
            if (reader.IsDBNull(reader.GetOrdinal("post_id"))) continue;

            Post post = new Post(
                Convert.ToInt64(reader["post_id"]),
                reader["title"].ToString(),
                reader["content"].ToString(),
                Convert.ToDateTime(reader["created_at"])
            );

            // AddPost metoda povezuje objavu sa korisnikom
            user.AddPost(post);
        }

        return user;
    }
}
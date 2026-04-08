using System.Data;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories;

public class CountryDbRepository : BaseDbRepository
{
    public Country Create(Country country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO countries(name, code)
            VALUES (@name, @code)
            RETURNING id";

        AddParameter(command, "@name", country.Name);
        AddParameter(command, "@code", country.Code);

        // ExecuteScalar vraća prvu kolonu prvog reda (id u ovom slučaju)
        long id = Convert.ToInt64(command.ExecuteScalar());

        return new Country(id, country.Name, country.Code);
    }

    public List<Country> GetAll()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, name, code
            FROM countries
            ORDER BY name";

        // ExecuteReader vraća IDataReader za čitanje više redova
        using IDataReader reader = command.ExecuteReader();

        List<Country> countries = new List<Country>();
        while (reader.Read())
            countries.Add(new Country(
                Convert.ToInt64(reader["id"]),
                reader["name"].ToString()!,
                reader["code"].ToString()!
            ));

        return countries;
    }

    public Country? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, code FROM countries WHERE id = @id";

        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Country(
                Convert.ToInt64(reader["id"]),
                reader["name"].ToString()!,
                reader["code"].ToString()!
            );
        }
        return null;
    }

    public void Update(Country country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE countries SET name = @name, code = @code WHERE id = @id";

        AddParameter(command, "@name", country.Name);
        AddParameter(command, "@code", country.Code);
        AddParameter(command, "@id", country.Id);

        command.ExecuteNonQuery();
    }

    // Briše državu samo ako nema povezanih gradova
    // Vraća true ako je brisanje uspelo, false ako je zabranjeno
    public bool Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        // Proveravam da li drzava ima gradove
        IDbCommand checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM cities WHERE country_id = @id";
        AddParameter(checkCmd, "@id", id);

        long count = Convert.ToInt64(checkCmd.ExecuteScalar());

        if (count > 0)
        {
            // Postoje gradovi
            return false;
        }

        // nema gradova, brisi
        IDbCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM countries WHERE id = @id";
        AddParameter(deleteCmd, "@id", id);

        deleteCmd.ExecuteNonQuery();
        return true;
    }
}
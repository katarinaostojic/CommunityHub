using System.Data;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories;

public class CityDbRepository : BaseDbRepository, ICityRepository
{
    public City Create(City city)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO cities(name, country_id)
            VALUES (@name, @country_id)
            RETURNING id";

        AddParameter(command, "@name", city.Name);
        AddParameter(command, "@country_id", city.Country.Id);

        // ExecuteScalar vraća prvu kolonu prvog reda (id u ovom slučaju)
        long id = Convert.ToInt64(command.ExecuteScalar());

        return new City(id, city.Name, city.Country);
    }

    public List<City> GetAll()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT ci.id, ci.name, co.id as country_id, co.name as country_name, co.code as country_code
            FROM cities ci 
            JOIN countries co ON ci.country_id = co.id
            ORDER BY ci.name";

        // ExecuteReader vraća IDataReader za čitanje više redova
        using IDataReader reader = command.ExecuteReader();

        List<City> cities = new List<City>();
        while (reader.Read())
        {
            Country country = new Country(
                Convert.ToInt64(reader["country_id"]),
                reader["country_name"].ToString()!,
                reader["country_code"].ToString()!
            );
            cities.Add(new City(Convert.ToInt64(reader["id"]), reader["name"].ToString()!, country));
        }

        return cities;
    }

    public City? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT ci.id, ci.name, co.id as country_id, co.name as country_name, co.code as country_code
            FROM cities ci 
            JOIN countries co ON ci.country_id = co.id
            WHERE ci.id = @id";

        AddParameter(command, "@id", id);

        using IDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new City(
                Convert.ToInt64(reader["id"]),
                reader["name"].ToString()!,
                new Country(
                    Convert.ToInt64(reader["country_id"]),
                    reader["country_name"].ToString()!,
                    reader["country_code"].ToString()!
                )
            );
        }

        return null; //ne postoji grad sa tim idem
    }

    public void Update(City city)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE cities 
            SET name = @name, country_id = @country_id 
            WHERE id = @id";

        AddParameter(command, "@name", city.Name);
        AddParameter(command, "@country_id", city.Country.Id);
        AddParameter(command, "@id", city.Id);

        command.ExecuteNonQuery();
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM cities WHERE id = @id";

        AddParameter(command, "@id", id);

        command.ExecuteNonQuery();
    }

    public List<City> GetByCountry(long countryId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT ci.id, ci.name, co.id as country_id, co.name as country_name, co.code as country_code
        FROM cities ci
        JOIN countries co ON ci.country_id = co.id
        WHERE ci.country_id = @countryId
        ORDER BY ci.name";

        AddParameter(command, "@countryId", countryId);

        using IDataReader reader = command.ExecuteReader();
        List<City> cities = new List<City>();
        while (reader.Read())
        {
            Country country = new Country(
                Convert.ToInt64(reader["country_id"]),
                reader["country_name"].ToString()!,
                reader["country_code"].ToString()!
            );
            cities.Add(new City(Convert.ToInt64(reader["id"]), reader["name"].ToString()!, country));
        }
        return cities;
    }
}
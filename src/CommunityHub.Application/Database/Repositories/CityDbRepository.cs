using System.Data;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories;

public class CityDbRepository
{
    public City Create(City city)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO cities(name, country_id)
            VALUES (@name, @country_id)
            RETURNING id";

        IDbDataParameter nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = city.Name;
        command.Parameters.Add(nameParam);

        IDbDataParameter countryIdParam = command.CreateParameter();
        countryIdParam.ParameterName = "@country_id";
        countryIdParam.Value = city.Country.Id;
        command.Parameters.Add(countryIdParam);

        // ExecuteScalar vraća prvu kolonu prvog reda (id u ovom slučaju)
        long id = Convert.ToInt64(command.ExecuteScalar());

        return new City(id, city.Name, city.Country);
    }

    public List<City> GetAll()
    {
        List<City> cities = new List<City>();
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT ci.id, ci.name, co.id as country_id, co.name as country_name, co.code as country_code
            FROM cities ci 
            JOIN countries co ON ci.country_id = co.id
            ORDER BY ci.name";

        // ExecuteReader vraća IDataReader za čitanje više redova
        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);
            string name = reader["name"].ToString();

            long countryId = Convert.ToInt64(reader["country_id"]);
            string countryName = reader["country_name"].ToString();
            string countryCode = reader["country_code"].ToString();

            Country country = new Country(countryId, countryName, countryCode);

            cities.Add(new City(id, name, country));
        }

        return cities;
    }

    public City? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT ci.id, ci.name, co.id as country_id, co.name as country_name, co.code as country_code
        FROM cities ci 
        JOIN countries co ON ci.country_id = co.id
        WHERE ci.id = @id";

        IDbDataParameter idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = id;
        command.Parameters.Add(idParam);

        using IDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new City(
                Convert.ToInt64(reader["id"]),
                reader["name"].ToString(),
                new Country(
                    Convert.ToInt64(reader["country_id"]),
                    reader["country_name"].ToString(),
                    reader["country_code"].ToString()
                )
            );
        }

        return null; //ne postoji grad sa tim idem
    }

    public void Update(City city)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE cities 
        SET name = @name, country_id = @country_id 
        WHERE id = @id";

        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = city.Name;
        command.Parameters.Add(nameParam);

        var countryIdParam = command.CreateParameter();
        countryIdParam.ParameterName = "@country_id";
        countryIdParam.Value = city.Country.Id;
        command.Parameters.Add(countryIdParam);

        var idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = city.Id;
        command.Parameters.Add(idParam);

        command.ExecuteNonQuery();
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM cities WHERE id = @id";

        IDbDataParameter idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = id;
        command.Parameters.Add(idParam);

        command.ExecuteNonQuery();
    }
}

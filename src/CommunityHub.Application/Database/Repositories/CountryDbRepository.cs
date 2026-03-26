using System.Data;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Database.Repositories;

public class CountryDbRepository
{
    public Country Create(Country country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO countries(name, code)
            VALUES (@name, @code)
            RETURNING id";

        IDbDataParameter nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = country.Name;
        command.Parameters.Add(nameParam);

        IDbDataParameter codeParam = command.CreateParameter();
        codeParam.ParameterName = "@code";
        codeParam.Value = country.Code;
        command.Parameters.Add(codeParam);

        // ExecuteScalar vraća prvu kolonu prvog reda (id u ovom slučaju)
        long id = Convert.ToInt64(command.ExecuteScalar());

        return new Country(id, country.Name, country.Code);
    }

    public List<Country> GetAll()
    {
        List<Country> countries = new List<Country>();
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT id, name, code
        FROM countries
        ORDER BY name";

        // ExecuteReader vraća IDataReader za čitanje više redova
        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            long id = Convert.ToInt64(reader["id"]);
            string name = reader["name"].ToString();
            string code = reader["code"].ToString();

            countries.Add(new Country(id, name, code));
        }

        return countries;
    }

    public Country? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, code FROM countries WHERE id = @id";

        var idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = id;
        command.Parameters.Add(idParam);

        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Country(
                Convert.ToInt64(reader["id"]),
                reader["name"].ToString(),
                reader["code"].ToString()
            );
        }
        return null;
    }

    public void Update(Country country)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        connection.Open();

        using IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE countries SET name = @name, code = @code WHERE id = @id";

        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = country.Name;
        command.Parameters.Add(nameParam);

        var codeParam = command.CreateParameter();
        codeParam.ParameterName = "@code";
        codeParam.Value = country.Code;
        command.Parameters.Add(codeParam);

        var idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = country.Id;
        command.Parameters.Add(idParam);

        command.ExecuteNonQuery();
    }

    // Briše državu samo ako nema povezanih gradova
    // Vraća true ako je brisanje uspelo, false ako je zabranjeno
    public bool Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        connection.Open();

        // Proveravam da li drzava ima gradove
        using IDbCommand checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM cities WHERE country_id = @id";

        var pIdCheck = checkCmd.CreateParameter();
        pIdCheck.ParameterName = "@id";
        pIdCheck.Value = id;
        checkCmd.Parameters.Add(pIdCheck);

        long count = Convert.ToInt64(checkCmd.ExecuteScalar());

        if (count > 0)
        {
            // Postoje gradovi
            return false;
        }

        // nema gradova, brisi
        using IDbCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM countries WHERE id = @id";

        var pIdDel = deleteCmd.CreateParameter();
        pIdDel.ParameterName = "@id";
        pIdDel.Value = id;
        deleteCmd.Parameters.Add(pIdDel);

        deleteCmd.ExecuteNonQuery();
        return true;
    }
}

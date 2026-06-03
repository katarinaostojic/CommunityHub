using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class DonationDbRepository : BaseDbRepository, IDonationRepository
{
    public List<DonationCategory> GetCategories()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, name FROM donation_categories ORDER BY name";

        using IDataReader reader = command.ExecuteReader();
        List<DonationCategory> categories = new();
        while (reader.Read())
            categories.Add(new DonationCategory(
                Convert.ToInt64(reader["id"]),
                reader["name"].ToString()!));
        return categories;
    }

    public void CreateDonation(long citizenId, long neighborhoodId, long categoryId, decimal amount)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO donations (citizen_id, neighborhood_id, category_id, amount, created_at)
            VALUES (@citizenId, @neighborhoodId, @categoryId, @amount, @createdAt)";

        AddParameter(command, "@citizenId", citizenId);
        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@categoryId", categoryId);
        AddParameter(command, "@amount", amount);
        AddParameter(command, "@createdAt", DateTime.UtcNow);
        command.ExecuteNonQuery();
    }

    public void UpdateNeighborhoodBudget(long neighborhoodId, decimal amount)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE neighborhoods 
            SET budget = budget + @amount 
            WHERE id = @neighborhoodId";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@amount", amount);
        command.ExecuteNonQuery();
    }

    public List<Donation> GetDonationsByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT d.id, d.citizen_id, d.neighborhood_id, d.category_id,
                   dc.name AS category_name, d.amount, d.created_at
            FROM donations d
            JOIN donation_categories dc ON d.category_id = dc.id
            WHERE d.neighborhood_id = @neighborhoodId
            ORDER BY d.created_at DESC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        using IDataReader reader = command.ExecuteReader();
        List<Donation> donations = new();
        while (reader.Read())
            donations.Add(MapDonation(reader));
        return donations;
    }

    public decimal GetBudgetByCategory(long neighborhoodId, long categoryId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COALESCE(SUM(d.amount), 0) - COALESCE(SUM(e.amount), 0)
            FROM donations d
            LEFT JOIN expenses e ON e.neighborhood_id = d.neighborhood_id 
                AND e.category_id = d.category_id
            WHERE d.neighborhood_id = @neighborhoodId
              AND d.category_id = @categoryId";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@categoryId", categoryId);
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    public List<Expense> GetExpensesByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT e.id, e.neighborhood_id, e.category_id,
                   dc.name AS category_name, e.amount, e.description, e.created_at
            FROM expenses e
            JOIN donation_categories dc ON e.category_id = dc.id
            WHERE e.neighborhood_id = @neighborhoodId
            ORDER BY e.created_at DESC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        using IDataReader reader = command.ExecuteReader();
        List<Expense> expenses = new();
        while (reader.Read())
            expenses.Add(MapExpense(reader));
        return expenses;
    }

    private Donation MapDonation(IDataReader reader)
    {
        return new Donation(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["citizen_id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            Convert.ToInt64(reader["category_id"]),
            reader["category_name"].ToString()!,
            Convert.ToDecimal(reader["amount"]),
            (DateOnly)reader["created_at"]
        );
    }

    private Expense MapExpense(IDataReader reader)
    {
        return new Expense(
            Convert.ToInt64(reader["id"]),
            Convert.ToInt64(reader["neighborhood_id"]),
            Convert.ToInt64(reader["category_id"]),
            reader["category_name"].ToString()!,
            Convert.ToDecimal(reader["amount"]),
            reader["description"].ToString()!,
            (DateOnly)reader["created_at"]
        );
    }
}

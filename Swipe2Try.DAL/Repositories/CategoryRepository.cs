using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.DAL.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string _connectionString;

    public CategoryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        var categories = new List<Category>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT CategoryID, Name, Photo FROM Categories", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var category = new Category
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CategoryID")), // Changed to GetInt32
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Photo = reader["Photo"]?.ToString()
                        };
                        categories.Add(category);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllCategoriesAsync: {ex.Message}");
        }

        return categories;
    }

    public async Task<Category?> GetCategoryByIdAsync(int id) // Changed string to int
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command =
                    new SqlCommand("SELECT CategoryID, Name, Photo FROM Categories WHERE CategoryID = @CategoryID",
                        connection);
                command.Parameters.AddWithValue("@CategoryID", id); // id is now int

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return new Category
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CategoryID")), // Changed to GetInt32
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Photo = reader["Photo"]?.ToString()
                        };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCategoryByIdAsync: {ex.Message}");
        }

        return null;
    }

    public async Task AddCategoryAsync(Category category)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // Removed CategoryID from INSERT statement as it's auto-incrementing
                // Added OUTPUT INSERTED.CategoryID to retrieve the generated ID
                var command =
                    new SqlCommand(
                        "INSERT INTO Categories (Name, Photo) OUTPUT INSERTED.CategoryID VALUES (@Name, @Photo)",
                        connection);
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Photo", (object?)category.Photo ?? DBNull.Value);

                // ExecuteScalarAsync to get the newly generated ID
                var newId = await command.ExecuteScalarAsync();
                if (newId != null && newId != DBNull.Value) category.Id = Convert.ToInt32(newId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddCategoryAsync: {ex.Message}");
            throw; // Re-throw to allow manager to handle
        }
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command =
                    new SqlCommand(
                        "UPDATE Categories SET Name = @Name, Photo = @Photo WHERE CategoryID = @CategoryID",
                        connection);
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Photo", (object?)category.Photo ?? DBNull.Value);
                command.Parameters.AddWithValue("@CategoryID", category.Id); // Id is now int

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateCategoryAsync: {ex.Message}");
            throw; // Re-throw to allow manager to handle
        }
    }

    public async Task DeleteCategoryAsync(int id) // Changed string to int
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("DELETE FROM Categories WHERE CategoryID = @CategoryID", connection);
                command.Parameters.AddWithValue("@CategoryID", id); // id is now int

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteCategoryAsync: {ex.Message}");
            throw; // Re-throw to allow manager to handle
        }
    }
}
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }        public async Task<List<Category>> GetAllCategoriesAsync()
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
                                Id = reader["CategoryID"]?.ToString() ?? string.Empty,
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

        public async Task<Category?> GetCategoryByIdAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT CategoryID, Name, Photo FROM Categories WHERE CategoryID = @CategoryID", connection);
                    command.Parameters.AddWithValue("@CategoryID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Category
                            {
                                Id = reader["CategoryID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Photo = reader["Photo"]?.ToString()
                            };
                        }
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
                    var command = new SqlCommand("INSERT INTO Categories (CategoryID, Name, Photo) VALUES (@CategoryID, @Name, @Photo)", connection);
                    command.Parameters.AddWithValue("@CategoryID", category.Id);
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Photo", (object?)category.Photo ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
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
                    var command = new SqlCommand("UPDATE Categories SET Name = @Name, Photo = @Photo WHERE CategoryID = @CategoryID", connection);
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Photo", (object?)category.Photo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CategoryID", category.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateCategoryAsync: {ex.Message}");
                throw; // Re-throw to allow manager to handle
            }
        }

        public async Task DeleteCategoryAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("DELETE FROM Categories WHERE CategoryID = @CategoryID", connection);
                    command.Parameters.AddWithValue("@CategoryID", id);

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
}

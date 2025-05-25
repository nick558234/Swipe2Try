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
    public class DishRepository : IDishRepository
    {
        private readonly string _connectionString;

        public DishRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<Dish>> GetAllDishesAsync()
        {
            var dishes = new List<Dish>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();                    var command = new SqlCommand(
                        @"SELECT DishID, Name, Description, HealthFactor, Photo, UserId, 'Not Available' AS Restaurant FROM Dishes",
                        connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {                            var dish = new Dish
                            {
                                Id = reader["DishID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                UserId = reader["UserId"]?.ToString() ?? string.Empty,
                                HealthFactor = reader["HealthFactor"]?.ToString(),
                                Photo = reader["Photo"]?.ToString(),
                                Restaurant = reader["Restaurant"]?.ToString()
                            };
                            dishes.Add(dish);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // Consider how to propagate this error to the UI or manager layer
                // For now, returning an empty list or re-throwing might be options
                // Or, add an error message to a list that can be returned
                Console.WriteLine($"Error in GetAllDishesAsync: {ex.Message}");
                // Depending on requirements, you might throw, return a custom error object, or an empty list with logged error.
            }            return dishes;
        }

        public async Task<List<Dish>> GetDishesByUserIdAsync(string userId)
        {
            var dishes = new List<Dish>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        @"SELECT DishID, Name, Description, HealthFactor, Photo, UserId, 'Not Available' AS Restaurant FROM Dishes WHERE UserId = @UserId",
                        connection);
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dish = new Dish
                            {
                                Id = reader["DishID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                UserId = reader["UserId"]?.ToString() ?? string.Empty,
                                HealthFactor = reader["HealthFactor"]?.ToString(),
                                Photo = reader["Photo"]?.ToString(),
                                Restaurant = reader["Restaurant"]?.ToString()
                            };
                            dishes.Add(dish);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDishesByUserIdAsync: {ex.Message}");
            }
            return dishes;
        }

        public async Task<Dish?> GetDishByIdAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT DishID, Name, Description, HealthFactor, Photo, UserId, 'Not Available' AS Restaurant FROM Dishes WHERE DishID = @DishID", connection);
                    command.Parameters.AddWithValue("@DishID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {                            return new Dish
                            {
                                Id = reader["DishID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                UserId = reader["UserId"]?.ToString() ?? string.Empty,
                                HealthFactor = reader["HealthFactor"]?.ToString(),
                                Photo = reader["Photo"]?.ToString(),
                                Restaurant = reader["Restaurant"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDishByIdAsync: {ex.Message}");
            }
            return null;
        }

        public async Task AddDishAsync(Dish dish)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();                    var command = new SqlCommand(
                        "INSERT INTO Dishes (DishID, Name, Description, HealthFactor, Photo, UserId) VALUES (@DishID, @Name, @Description, @HealthFactor, @Photo, @UserId)",
                        connection);
                    command.Parameters.AddWithValue("@DishID", dish.Id);
                    command.Parameters.AddWithValue("@Name", dish.Name);
                    command.Parameters.AddWithValue("@Description", dish.Description);
                    command.Parameters.AddWithValue("@HealthFactor", (object?)dish.HealthFactor ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Photo", (object?)dish.Photo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", dish.UserId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddDishAsync: {ex.Message}");
                // Handle or re-throw the exception as appropriate
            }
        }

        public async Task UpdateDishAsync(Dish dish)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();                    var command = new SqlCommand(
                        "UPDATE Dishes SET Name = @Name, Description = @Description, HealthFactor = @HealthFactor, Photo = @Photo, UserId = @UserId WHERE DishID = @DishID",
                        connection);
                    command.Parameters.AddWithValue("@DishID", dish.Id);
                    command.Parameters.AddWithValue("@Name", dish.Name);
                    command.Parameters.AddWithValue("@Description", dish.Description);
                    command.Parameters.AddWithValue("@HealthFactor", (object?)dish.HealthFactor ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Photo", (object?)dish.Photo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", dish.UserId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateDishAsync: {ex.Message}");
            }
        }

        public async Task DeleteDishAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("DELETE FROM Dishes WHERE DishID = @DishID", connection);
                    command.Parameters.AddWithValue("@DishID", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteDishAsync: {ex.Message}");
            }
        }
    }
}

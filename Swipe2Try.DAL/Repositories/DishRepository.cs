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

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT DishID, Name, Description, HealthFactor, Photo, 'Not Available' AS Restaurant FROM Dishes",
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dish = new Dish
                        {
                            Id = reader["DishID"]?.ToString() ?? string.Empty,
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Description = reader["Description"]?.ToString() ?? string.Empty,
                            HealthFactor = reader["HealthFactor"]?.ToString(),
                            Photo = reader["Photo"]?.ToString(),
                            Restaurant = reader["Restaurant"]?.ToString()
                        };
                        dishes.Add(dish);
                    }
                }
            }

            return dishes;
        }

        public async Task<Dish?> GetDishByIdAsync(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT DishID, Name, Description, 
                      'Not Available' AS Restaurant 
                      FROM Dishes WHERE DishID = @Id", 
                    connection);

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Dish
                        {
                            Id = reader["DishID"]?.ToString() ?? string.Empty,
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Description = reader["Description"]?.ToString() ?? string.Empty,
                            Restaurant = reader["Restaurant"]?.ToString()
                        };
                    }
                }
            }

            return null;
        }

        public async Task AddDishAsync(Dish dish)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "INSERT INTO Dishes (DishID, Name, Description, HealthFactor, Photo) VALUES (@Id, @Name, @Description, @HealthFactor, @Photo)",
                    connection);

                command.Parameters.AddWithValue("@Id", dish.Id ?? Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@Name", dish.Name);
                command.Parameters.AddWithValue("@Description", dish.Description);
                command.Parameters.AddWithValue("@HealthFactor", (object?)dish.HealthFactor ?? DBNull.Value);
                command.Parameters.AddWithValue("@Photo", (object?)dish.Photo ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateDishAsync(Dish dish)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "UPDATE Dishes SET Name = @Name, Description = @Description, HealthFactor = @HealthFactor, Photo = @Photo WHERE DishID = @Id",
                    connection);

                command.Parameters.AddWithValue("@Id", dish.Id);
                command.Parameters.AddWithValue("@Name", dish.Name);
                command.Parameters.AddWithValue("@Description", dish.Description);
                command.Parameters.AddWithValue("@HealthFactor", (object?)dish.HealthFactor ?? DBNull.Value);
                command.Parameters.AddWithValue("@Photo", (object?)dish.Photo ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteDishAsync(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "DELETE FROM Dishes WHERE DishID = @Id",
                    connection);

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}

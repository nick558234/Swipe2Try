using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.DAL.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly string _connectionString;

    public RestaurantRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<Restaurant>> GetAllRestaurantsAsync()
    {
        var restaurants = new List<Restaurant>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT RestaurantID, Name, Location, UserId FROM Restaurants",
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var restaurant = new Restaurant
                        {
                            Id = reader.GetInt32("RestaurantID"),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Location = reader["Location"]?.ToString() ?? string.Empty,
                            UserId = reader["UserId"]?.ToString() ?? string.Empty
                        };
                        restaurants.Add(restaurant);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllRestaurantsAsync: {ex.Message}");
        }

        return restaurants;
    }

    public async Task<List<Restaurant>> GetRestaurantsByUserIdAsync(string userId)
    {
        var restaurants = new List<Restaurant>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT RestaurantID, Name, Location, UserId FROM Restaurants WHERE UserId = @UserId",
                    connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var restaurant = new Restaurant
                        {
                            Id = reader.GetInt32("RestaurantID"),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Location = reader["Location"]?.ToString() ?? string.Empty,
                            UserId = reader["UserId"]?.ToString() ?? string.Empty
                        };
                        restaurants.Add(restaurant);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRestaurantsByUserIdAsync: {ex.Message}");
        }

        return restaurants;
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command =
                    new SqlCommand(
                        "SELECT RestaurantID, Name, Location, UserId FROM Restaurants WHERE RestaurantID = @RestaurantID",
                        connection);
                command.Parameters.AddWithValue("@RestaurantID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return new Restaurant
                        {
                            Id = reader.GetInt32("RestaurantID"),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Location = reader["Location"]?.ToString() ?? string.Empty,
                            UserId = reader["UserId"]?.ToString() ?? string.Empty
                        };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRestaurantByIdAsync: {ex.Message}");
        }

        return null;
    }

    public async Task AddRestaurantAsync(Restaurant restaurant)
    {
        try
        {
            // Defensive programming: ensure no null values
            if (restaurant == null)
                throw new ArgumentNullException(nameof(restaurant));
            if (string.IsNullOrEmpty(restaurant.Name))
                throw new ArgumentException("Restaurant name cannot be null or empty", nameof(restaurant.Name));
            if (string.IsNullOrEmpty(restaurant.Location))
                throw new ArgumentException("Restaurant location cannot be null or empty",
                    nameof(restaurant.Location));
            if (string.IsNullOrEmpty(restaurant.UserId))
                throw new ArgumentException("Restaurant UserId cannot be null or empty", nameof(restaurant.UserId));

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "INSERT INTO Restaurants (Name, Location, UserId) VALUES (@Name, @Location, @UserId)",
                    connection);
                command.Parameters.AddWithValue("@Name", restaurant.Name);
                command.Parameters.AddWithValue("@Location", restaurant.Location);
                command.Parameters.AddWithValue("@UserId", restaurant.UserId);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddRestaurantAsync: {ex.Message}");
            throw; // Re-throw to ensure the error propagates properly
        }
    }

    public async Task UpdateRestaurantAsync(Restaurant restaurant)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "UPDATE Restaurants SET Name = @Name, Location = @Location, UserId = @UserId WHERE RestaurantID = @RestaurantID",
                    connection);
                command.Parameters.AddWithValue("@RestaurantID", restaurant.Id);
                command.Parameters.AddWithValue("@Name", restaurant.Name);
                command.Parameters.AddWithValue("@Location", restaurant.Location);
                command.Parameters.AddWithValue("@UserId", restaurant.UserId);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateRestaurantAsync: {ex.Message}");
        }
    }

    public async Task DeleteRestaurantAsync(int id)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("DELETE FROM Restaurants WHERE RestaurantID = @RestaurantID",
                    connection);
                command.Parameters.AddWithValue("@RestaurantID", id);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteRestaurantAsync: {ex.Message}");
        }
    }
}
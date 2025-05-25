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
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly string _connectionString;

        public RestaurantRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
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
                                Id = reader["RestaurantID"]?.ToString() ?? string.Empty,
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
                                Id = reader["RestaurantID"]?.ToString() ?? string.Empty,
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

        public async Task<Restaurant?> GetRestaurantByIdAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT RestaurantID, Name, Location, UserId FROM Restaurants WHERE RestaurantID = @RestaurantID", connection);
                    command.Parameters.AddWithValue("@RestaurantID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Restaurant
                            {
                                Id = reader["RestaurantID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Location = reader["Location"]?.ToString() ?? string.Empty,
                                UserId = reader["UserId"]?.ToString() ?? string.Empty
                            };
                        }
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        "INSERT INTO Restaurants (RestaurantID, Name, Location, UserId) VALUES (@RestaurantID, @Name, @Location, @UserId)",
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
                Console.WriteLine($"Error in AddRestaurantAsync: {ex.Message}");
                // Handle or re-throw the exception as appropriate
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

        public async Task DeleteRestaurantAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("DELETE FROM Restaurants WHERE RestaurantID = @RestaurantID", connection);
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
}

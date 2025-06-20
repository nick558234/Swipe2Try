using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.DAL.Repositories;

public class DishRestaurantRepository : IDishRestaurantRepository
{
    private readonly string _connectionString;

    public DishRestaurantRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<DishRestaurant>> GetAllDishRestaurantsAsync()
    {
        var dishRestaurants = new List<DishRestaurant>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();                var command = new SqlCommand(
                    @"SELECT Dish_ID, Restaurant_ID FROM DISHRESTAURANT",
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dishRestaurant = new DishRestaurant
                        {
                            DishId = reader.GetInt32("Dish_ID"),
                            RestaurantId = reader.GetInt32("Restaurant_ID")
                        };
                        dishRestaurants.Add(dishRestaurant);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllDishRestaurantsAsync: {ex.Message}");
        }

        return dishRestaurants;
    }

    public async Task<List<DishRestaurant>> GetDishRestaurantsByDishIdAsync(int dishId)
    {
        var dishRestaurants = new List<DishRestaurant>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();                var command = new SqlCommand(
                    @"SELECT Dish_ID, Restaurant_ID FROM DISHRESTAURANT WHERE Dish_ID = @DishId",
                    connection);
                command.Parameters.AddWithValue("@DishId", dishId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dishRestaurant = new DishRestaurant
                        {
                            DishId = reader.GetInt32("Dish_ID"),
                            RestaurantId = reader.GetInt32("Restaurant_ID")
                        };
                        dishRestaurants.Add(dishRestaurant);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDishRestaurantsByDishIdAsync: {ex.Message}");
        }

        return dishRestaurants;
    }

    public async Task<List<DishRestaurant>> GetDishRestaurantsByRestaurantIdAsync(int restaurantId)
    {
        var dishRestaurants = new List<DishRestaurant>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();                var command = new SqlCommand(
                    @"SELECT Dish_ID, Restaurant_ID FROM DISHRESTAURANT WHERE Restaurant_ID = @RestaurantId",
                    connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dishRestaurant = new DishRestaurant
                        {
                            DishId = reader.GetInt32("Dish_ID"),
                            RestaurantId = reader.GetInt32("Restaurant_ID")
                        };
                        dishRestaurants.Add(dishRestaurant);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDishRestaurantsByRestaurantIdAsync: {ex.Message}");
        }

        return dishRestaurants;
    }

    public async Task<List<Restaurant>> GetRestaurantsByDishIdAsync(int dishId)
    {
        var restaurants = new List<Restaurant>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT r.RestaurantID, r.Name, r.Location, r.UserId 
                    FROM Restaurants r
                    INNER JOIN DISHRESTAURANT dr ON r.RestaurantID = dr.Restaurant_ID
                    WHERE dr.Dish_ID = @DishId", connection);
                command.Parameters.AddWithValue("@DishId", dishId);

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
            Console.WriteLine($"Error in GetRestaurantsByDishIdAsync: {ex.Message}");
        }

        return restaurants;
    }

    public async Task<List<Dish>> GetDishesByRestaurantIdAsync(int restaurantId)
    {
        var dishes = new List<Dish>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT d.DishID, d.Name, d.Description, d.HealthFactor, d.Photo, d.UserId 
                    FROM Dishes d
                    INNER JOIN DISHRESTAURANT dr ON d.DishID = dr.Dish_ID
                    WHERE dr.Restaurant_ID = @RestaurantId", connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dish = new Dish
                        {
                            Id = reader.GetInt32("DishID"),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Description = reader["Description"]?.ToString() ?? string.Empty,
                            UserId = reader["UserId"]?.ToString() ?? string.Empty,
                            HealthFactor = reader["HealthFactor"] == DBNull.Value
                                ? null
                                : (int?)reader.GetInt32("HealthFactor"),
                            Photo = reader["Photo"]?.ToString()
                        };
                        dishes.Add(dish);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDishesByRestaurantIdAsync: {ex.Message}");
        }

        return dishes;
    }

    public async Task<bool> LinkDishToRestaurantAsync(int dishId, int restaurantId)
    {
        try
        {
            // Check if the link already exists
            if (await IsDishLinkedToRestaurantAsync(dishId, restaurantId))
            {
                return true; // Already linked
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"INSERT INTO DISHRESTAURANT (Dish_ID, Restaurant_ID) VALUES (@DishId, @RestaurantId)",
                    connection);
                command.Parameters.AddWithValue("@DishId", dishId);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LinkDishToRestaurantAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UnlinkDishFromRestaurantAsync(int dishId, int restaurantId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"DELETE FROM DISHRESTAURANT WHERE Dish_ID = @DishId AND Restaurant_ID = @RestaurantId",
                    connection);
                command.Parameters.AddWithValue("@DishId", dishId);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UnlinkDishFromRestaurantAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsDishLinkedToRestaurantAsync(int dishId, int restaurantId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT COUNT(*) FROM DISHRESTAURANT WHERE Dish_ID = @DishId AND Restaurant_ID = @RestaurantId",
                    connection);
                command.Parameters.AddWithValue("@DishId", dishId);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);                var result = await command.ExecuteScalarAsync();
                int count = result != null ? (int)result : 0;
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in IsDishLinkedToRestaurantAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<int> LinkDishesByOwnershipAsync()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    INSERT INTO DISHRESTAURANT (Dish_ID, Restaurant_ID)
                    SELECT d.DishID, r.RestaurantID
                    FROM Dishes d
                    INNER JOIN Restaurants r ON d.UserId = r.UserId
                    WHERE NOT EXISTS (
                        SELECT 1 FROM DISHRESTAURANT dr 
                        WHERE dr.Dish_ID = d.DishID AND dr.Restaurant_ID = r.RestaurantID
                    )", connection);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LinkDishesByOwnershipAsync: {ex.Message}");
            return 0;
        }
    }

    public async Task<Dish?> GetDishWithRestaurantsAsync(int dishId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                // First get the dish
                var dishCommand = new SqlCommand(
                    @"SELECT DishID, Name, Description, HealthFactor, Photo, UserId FROM Dishes WHERE DishID = @DishId",
                    connection);
                dishCommand.Parameters.AddWithValue("@DishId", dishId);

                Dish? dish = null;
                using (var reader = await dishCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        dish = new Dish
                        {
                            Id = reader.GetInt32("DishID"),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Description = reader["Description"]?.ToString() ?? string.Empty,
                            UserId = reader["UserId"]?.ToString() ?? string.Empty,
                            HealthFactor = reader["HealthFactor"] == DBNull.Value
                                ? null
                                : (int?)reader.GetInt32("HealthFactor"),
                            Photo = reader["Photo"]?.ToString()
                        };
                    }
                }

                if (dish != null)
                {
                    // Get associated restaurants
                    dish.Restaurants = await GetRestaurantsByDishIdAsync(dishId);
                    
                    // Set the Restaurant property to the first restaurant name if any exist
                    if (dish.Restaurants.Any())
                    {
                        dish.Restaurant = dish.Restaurants.First().Name;
                    }
                }

                return dish;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDishWithRestaurantsAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<Restaurant?> GetRestaurantWithDishesAsync(int restaurantId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                // First get the restaurant
                var restaurantCommand = new SqlCommand(
                    @"SELECT RestaurantID, Name, Location, UserId FROM Restaurants WHERE RestaurantID = @RestaurantId",
                    connection);
                restaurantCommand.Parameters.AddWithValue("@RestaurantId", restaurantId);

                Restaurant? restaurant = null;
                using (var reader = await restaurantCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        restaurant = new Restaurant
                        {
                            Id = reader.GetInt32("RestaurantID"),
                            Name = reader["Name"]?.ToString() ?? string.Empty,
                            Location = reader["Location"]?.ToString() ?? string.Empty,
                            UserId = reader["UserId"]?.ToString() ?? string.Empty
                        };
                    }
                }

                if (restaurant != null)
                {
                    // Get associated dishes
                    restaurant.Dishes = await GetDishesByRestaurantIdAsync(restaurantId);
                }

                return restaurant;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRestaurantWithDishesAsync: {ex.Message}");
            return null;
        }
    }
}

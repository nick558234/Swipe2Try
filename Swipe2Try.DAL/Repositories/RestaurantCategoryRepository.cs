using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.DAL.Repositories;

public class RestaurantCategoryRepository : IRestaurantCategoryRepository
{
    private readonly string _connectionString;

    public RestaurantCategoryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<RestaurantCategory>> GetAllRestaurantCategoriesAsync()
    {
        var restaurantCategories = new List<RestaurantCategory>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT RestaurantID, CategoryID FROM RestaurantCategories",
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var restaurantCategory = new RestaurantCategory
                        {
                            RestaurantId = int.Parse(reader["RestaurantID"].ToString() ?? "0"),
                            CategoryId = reader.GetInt32("CategoryID")
                        };
                        restaurantCategories.Add(restaurantCategory);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllRestaurantCategoriesAsync: {ex.Message}");
        }

        return restaurantCategories;
    }

    public async Task<List<Category>> GetCategoriesByRestaurantIdAsync(int restaurantId)
    {
        var categories = new List<Category>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT c.CategoryID, c.Name, c.Photo 
                    FROM Categories c
                    INNER JOIN RestaurantCategories rc ON c.CategoryID = rc.CategoryID
                    WHERE rc.RestaurantID = @RestaurantId", connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId.ToString());

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var category = new Category
                        {
                            Id = reader.GetInt32("CategoryID"),
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
            Console.WriteLine($"Error in GetCategoriesByRestaurantIdAsync: {ex.Message}");
        }

        return categories;
    }

    public async Task<List<Restaurant>> GetRestaurantsByCategoryIdAsync(int categoryId)
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
                    INNER JOIN RestaurantCategories rc ON r.RestaurantID = rc.RestaurantID
                    WHERE rc.CategoryID = @CategoryId", connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

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
            Console.WriteLine($"Error in GetRestaurantsByCategoryIdAsync: {ex.Message}");
        }

        return restaurants;
    }

    public async Task<bool> LinkRestaurantToCategoryAsync(int restaurantId, int categoryId)
    {
        try
        {
            // Check if the link already exists
            if (await IsRestaurantLinkedToCategoryAsync(restaurantId, categoryId))
            {
                return true; // Already linked
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"INSERT INTO RestaurantCategories (RestaurantID, CategoryID) VALUES (@RestaurantId, @CategoryId)",
                    connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId.ToString());
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LinkRestaurantToCategoryAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UnlinkRestaurantFromCategoryAsync(int restaurantId, int categoryId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"DELETE FROM RestaurantCategories WHERE RestaurantID = @RestaurantId AND CategoryID = @CategoryId",
                    connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId.ToString());
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UnlinkRestaurantFromCategoryAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsRestaurantLinkedToCategoryAsync(int restaurantId, int categoryId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT COUNT(*) FROM RestaurantCategories WHERE RestaurantID = @RestaurantId AND CategoryID = @CategoryId",
                    connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId.ToString());
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                var result = await command.ExecuteScalarAsync();
                int count = result != null ? (int)result : 0;
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in IsRestaurantLinkedToCategoryAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<Restaurant?> GetRestaurantWithCategoriesAsync(int restaurantId)
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
                    // Get associated categories
                    restaurant.Categories = await GetCategoriesByRestaurantIdAsync(restaurantId);
                }

                return restaurant;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRestaurantWithCategoriesAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateRestaurantCategoriesAsync(int restaurantId, List<int> categoryIds)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First, remove all existing categories for this restaurant
                        var deleteCommand = new SqlCommand(
                            @"DELETE FROM RestaurantCategories WHERE RestaurantID = @RestaurantId",
                            connection, transaction);
                        deleteCommand.Parameters.AddWithValue("@RestaurantId", restaurantId.ToString());
                        await deleteCommand.ExecuteNonQueryAsync();

                        // Then, add the new categories
                        foreach (var categoryId in categoryIds)
                        {
                            var insertCommand = new SqlCommand(
                                @"INSERT INTO RestaurantCategories (RestaurantID, CategoryID) VALUES (@RestaurantId, @CategoryId)",
                                connection, transaction);
                            insertCommand.Parameters.AddWithValue("@RestaurantId", restaurantId.ToString());
                            insertCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                            await insertCommand.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateRestaurantCategoriesAsync: {ex.Message}");
            return false;
        }
    }
}

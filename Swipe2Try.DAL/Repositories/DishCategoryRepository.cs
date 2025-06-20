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
    public class DishCategoryRepository : IDishCategoryRepository
    {
        private readonly string _connectionString;

        public DishCategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<DishCategory>> GetAllDishCategoriesAsync()
        {
            var dishCategories = new List<DishCategory>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        @"SELECT DishID, CategoryID FROM DishCategories",
                        connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dishCategory = new DishCategory
                            {
                                DishId = reader.GetInt32("DishID"),
                                CategoryId = reader.GetInt32("CategoryID")
                            };
                            dishCategories.Add(dishCategory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllDishCategoriesAsync: {ex.Message}");
            }
            return dishCategories;
        }

        public async Task<List<Category>> GetCategoriesByDishIdAsync(int dishId)
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
                        INNER JOIN DishCategories dc ON c.CategoryID = dc.CategoryID
                        WHERE dc.DishID = @DishId", connection);
                    command.Parameters.AddWithValue("@DishId", dishId);

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
                Console.WriteLine($"Error in GetCategoriesByDishIdAsync: {ex.Message}");
            }
            return categories;
        }

        public async Task<List<Dish>> GetDishesByCategoryIdAsync(int categoryId)
        {
            var dishes = new List<Dish>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT d.DishID, d.Name, d.Description, d.Price, d.HealthFactor, d.Photo, d.UserId
                        FROM DISHES d
                        INNER JOIN DishCategories dc ON d.DishID = dc.DishID
                        WHERE dc.CategoryID = @CategoryId", connection);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var dish = new Dish
                            {
                                Id = reader.GetInt32("DishID"),
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                HealthFactor = reader["HealthFactor"] as int?,
                                Photo = reader["Photo"]?.ToString(),
                                UserId = reader["UserId"]?.ToString() ?? string.Empty
                            };
                            dishes.Add(dish);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDishesByCategoryIdAsync: {ex.Message}");
            }
            return dishes;
        }

        public async Task<bool> LinkDishToCategoryAsync(int dishId, int categoryId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        @"INSERT INTO DishCategories (DishID, CategoryID) VALUES (@DishId, @CategoryId)",
                        connection);
                    command.Parameters.AddWithValue("@DishId", dishId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LinkDishToCategoryAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnlinkDishFromCategoryAsync(int dishId, int categoryId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        @"DELETE FROM DishCategories WHERE DishID = @DishId AND CategoryID = @CategoryId",
                        connection);
                    command.Parameters.AddWithValue("@DishId", dishId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UnlinkDishFromCategoryAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsDishLinkedToCategoryAsync(int dishId, int categoryId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        @"SELECT COUNT(*) FROM DishCategories WHERE DishID = @DishId AND CategoryID = @CategoryId",
                        connection);
                    command.Parameters.AddWithValue("@DishId", dishId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IsDishLinkedToCategoryAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<Dish?> GetDishWithCategoriesAsync(int dishId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // First get the dish
                    var dishCommand = new SqlCommand(
                        @"SELECT DishID, Name, Description, Price, HealthFactor, Photo, UserId 
                          FROM DISHES WHERE DishID = @DishId", connection);
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
                                HealthFactor = reader["HealthFactor"] as int?,
                                Photo = reader["Photo"]?.ToString(),
                                UserId = reader["UserId"]?.ToString() ?? string.Empty
                            };
                        }
                    }

                    if (dish != null)
                    {
                        // Then get the categories
                        var categories = await GetCategoriesByDishIdAsync(dishId);
                        dish.Categories = categories;
                    }

                    return dish;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDishWithCategoriesAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateDishCategoriesAsync(int dishId, List<int> categoryIds)
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
                            // Remove existing categories
                            var deleteCommand = new SqlCommand(
                                @"DELETE FROM DishCategories WHERE DishID = @DishId",
                                connection, transaction);
                            deleteCommand.Parameters.AddWithValue("@DishId", dishId);
                            await deleteCommand.ExecuteNonQueryAsync();

                            // Add new categories
                            foreach (var categoryId in categoryIds)
                            {
                                var insertCommand = new SqlCommand(
                                    @"INSERT INTO DishCategories (DishID, CategoryID) VALUES (@DishId, @CategoryId)",
                                    connection, transaction);
                                insertCommand.Parameters.AddWithValue("@DishId", dishId);
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
                Console.WriteLine($"Error in UpdateDishCategoriesAsync: {ex.Message}");
                return false;
            }
        }
    }
}

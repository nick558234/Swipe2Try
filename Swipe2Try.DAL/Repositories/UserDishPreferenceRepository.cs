using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;

namespace Swipe2Try.DAL.Repositories;

public class UserDishPreferenceRepository : IUserDishPreferenceRepository
{
    private readonly string _connectionString;

    public UserDishPreferenceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<UserDishPreference?> GetUserDishPreferenceAsync(string userId, int dishId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT Id, UserId, DishId, IsLiked, CreatedAt, UpdatedAt 
                      FROM UserDishPreferences 
                      WHERE UserId = @UserId AND DishId = @DishId",
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@DishId", dishId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserDishPreference
                        {
                            Id = reader.GetInt32("Id"),
                            UserId = reader.GetString("UserId"),
                            DishId = reader.GetInt32("DishId"),
                            IsLiked = reader.GetBoolean("IsLiked"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value 
                                ? null 
                                : reader.GetDateTime("UpdatedAt")
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving user dish preference: {ex.Message}", ex);
        }

        return null;
    }

    public async Task<List<UserDishPreference>> GetUserPreferencesAsync(string userId)
    {
        var preferences = new List<UserDishPreference>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT Id, UserId, DishId, IsLiked, CreatedAt, UpdatedAt 
                      FROM UserDishPreferences 
                      WHERE UserId = @UserId
                      ORDER BY CreatedAt DESC",
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        preferences.Add(new UserDishPreference
                        {
                            Id = reader.GetInt32("Id"),
                            UserId = reader.GetString("UserId"),
                            DishId = reader.GetInt32("DishId"),
                            IsLiked = reader.GetBoolean("IsLiked"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value 
                                ? null 
                                : reader.GetDateTime("UpdatedAt")
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving user preferences: {ex.Message}", ex);
        }

        return preferences;
    }

    public async Task<List<UserDishPreference>> GetDishPreferencesAsync(int dishId)
    {
        var preferences = new List<UserDishPreference>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT Id, UserId, DishId, IsLiked, CreatedAt, UpdatedAt 
                      FROM UserDishPreferences 
                      WHERE DishId = @DishId
                      ORDER BY CreatedAt DESC",
                    connection);

                command.Parameters.AddWithValue("@DishId", dishId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        preferences.Add(new UserDishPreference
                        {
                            Id = reader.GetInt32("Id"),
                            UserId = reader.GetString("UserId"),
                            DishId = reader.GetInt32("DishId"),
                            IsLiked = reader.GetBoolean("IsLiked"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value 
                                ? null 
                                : reader.GetDateTime("UpdatedAt")
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving dish preferences: {ex.Message}", ex);
        }

        return preferences;
    }

    public async Task AddUserDishPreferenceAsync(UserDishPreference preference)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"INSERT INTO UserDishPreferences (UserId, DishId, IsLiked, CreatedAt)
                      VALUES (@UserId, @DishId, @IsLiked, @CreatedAt);
                      SELECT CAST(SCOPE_IDENTITY() as int);",
                    connection);

                command.Parameters.AddWithValue("@UserId", preference.UserId);
                command.Parameters.AddWithValue("@DishId", preference.DishId);
                command.Parameters.AddWithValue("@IsLiked", preference.IsLiked);
                command.Parameters.AddWithValue("@CreatedAt", preference.CreatedAt);

                var newId = await command.ExecuteScalarAsync();
                if (newId != null)
                {
                    preference.Id = (int)newId;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding user dish preference: {ex.Message}", ex);
        }
    }

    public async Task UpdateUserDishPreferenceAsync(UserDishPreference preference)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"UPDATE UserDishPreferences 
                      SET IsLiked = @IsLiked, UpdatedAt = @UpdatedAt
                      WHERE UserId = @UserId AND DishId = @DishId",
                    connection);

                command.Parameters.AddWithValue("@UserId", preference.UserId);
                command.Parameters.AddWithValue("@DishId", preference.DishId);
                command.Parameters.AddWithValue("@IsLiked", preference.IsLiked);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating user dish preference: {ex.Message}", ex);
        }
    }

    public async Task DeleteUserDishPreferenceAsync(string userId, int dishId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"DELETE FROM UserDishPreferences 
                      WHERE UserId = @UserId AND DishId = @DishId",
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@DishId", dishId);

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting user dish preference: {ex.Message}", ex);
        }
    }
}

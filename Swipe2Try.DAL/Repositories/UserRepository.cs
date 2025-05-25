using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Threading.Tasks;

namespace Swipe2Try.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found.");
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT UserID, Name, Email, Password, RoleID FROM dbo.USERS WHERE LOWER(Email) = LOWER(@Email)",
                        connection);

                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserID = reader["UserID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Email = reader["Email"]?.ToString() ?? string.Empty,
                                Password = reader["Password"]?.ToString() ?? string.Empty,
                                RoleID = reader["RoleID"]?.ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserByEmailAsync: {ex.Message}");
                // Propagate or log the exception as per application's error handling strategy
                // For now, returning null indicates failure or user not found
            }
            return null;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        @"INSERT INTO dbo.USERS (UserID, Name, Email, Password, RoleID)
                          VALUES (@UserID, @Name, @Email, @Password, @RoleID)",
                        connection);

                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateUserAsync: {ex.Message}");
                return false; // Indicate failure
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT COUNT(1) FROM dbo.USERS WHERE LOWER(Email) = LOWER(@Email)", connection);
                    command.Parameters.AddWithValue("@Email", email);
                    var result = await command.ExecuteScalarAsync();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EmailExistsAsync: {ex.Message}");
                return false; // Indicate failure or error
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT UserID, Name, Email, RoleID FROM dbo.USERS", connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                UserID = reader["UserID"]?.ToString() ?? string.Empty,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Email = reader["Email"]?.ToString() ?? string.Empty,
                                RoleID = reader["RoleID"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllUsersAsync: {ex.Message}");
                // Depending on requirements, you might throw, or return an empty list with logged error.
            }
            return users;
        }
    }
}

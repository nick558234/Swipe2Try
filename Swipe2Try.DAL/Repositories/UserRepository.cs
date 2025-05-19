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
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> GetUserByEmailAsync(string email)
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
                            UserID = reader["UserID"].ToString(),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString(),
                            RoleID = reader["RoleID"].ToString()
                        };
                    }
                }
            }

            return null;
        }

        public async Task<bool> CreateUserAsync(User user)
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

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "SELECT COUNT(1) FROM dbo.USERS WHERE Email = @Email",
                    connection);
                
                command.Parameters.AddWithValue("@Email", email);

                var result = (int)await command.ExecuteScalarAsync();
                return result > 0;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"SELECT u.UserID, u.Name, u.Email, u.Password, u.RoleID, r.RoleName 
                      FROM Users u
                      JOIN Roles r ON u.RoleID = r.RoleID
                      ORDER BY u.Name",
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new User
                        {
                            UserID = reader["UserID"].ToString(),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString(), // In a real app, you wouldn't return passwords
                            RoleID = reader["RoleID"].ToString(),
                            // Add additional property for display - assuming User has a RoleName property or you add it
                            // If not in the User model, you can create a UserViewModel with this property
                        };
                        users.Add(user);
                    }
                }
            }

            return users;
        }
    }
}

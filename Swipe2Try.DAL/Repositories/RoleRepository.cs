using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = new List<Role>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT RoleID, RoleName FROM dbo.ROLES", connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(new Role
                            {
                                RoleID = reader["RoleID"]?.ToString() ?? string.Empty,
                                RoleName = reader["RoleName"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllRolesAsync: {ex.Message}");
            }
            return roles;
        }

        public async Task<Role?> GetRoleByIdAsync(string roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT RoleID, RoleName FROM dbo.ROLES WHERE RoleID = @RoleID", connection);
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Role
                            {
                                RoleID = reader["RoleID"]?.ToString() ?? string.Empty,
                                RoleName = reader["RoleName"]?.ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRoleByIdAsync: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> RoleExistsAsync(string roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT COUNT(1) FROM dbo.ROLES WHERE RoleID = @RoleID", connection);
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    var result = await command.ExecuteScalarAsync();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RoleExistsAsync: {ex.Message}");
                return false;
            }
        }
    }
}
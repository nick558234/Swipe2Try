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
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Dish>> GetAllDishesAsync()
        {
            var dishes = new List<Dish>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // Be explicit about which columns we want
                var command = new SqlCommand(
                    @"SELECT DishID, Name, Description, 
                      'Not Available' AS Restaurant 
                      FROM Dishes", 
                    connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dish = new Dish
                        {
                            Id = reader["DishID"].ToString(),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Restaurant = reader["Restaurant"].ToString()
                        };
                        dishes.Add(dish);
                    }
                }
            }

            return dishes;
        }

        public async Task AddDishAsync(Dish dish)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "INSERT INTO Dishes (Name, Description) VALUES (@Name, @Description)",
                    connection);

                command.Parameters.AddWithValue("@Name", dish.Name);
                command.Parameters.AddWithValue("@Description", dish.Description);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}

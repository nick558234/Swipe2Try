using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces;

public interface IDishRepository
{
    Task<List<Dish>> GetAllDishesAsync();
    Task<List<Dish>> GetDishesByUserIdAsync(string userId);
    Task<Dish?> GetDishByIdAsync(int id);
    Task AddDishAsync(Dish dish);
    Task UpdateDishAsync(Dish dish);
    Task DeleteDishAsync(int id);
}
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces
{
    public interface IDishRepository
    {
        Task<List<Dish>> GetAllDishesAsync();
        Task AddDishAsync(Dish dish);
    }
} 
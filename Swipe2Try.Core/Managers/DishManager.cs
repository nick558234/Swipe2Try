using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Managers
{
    public class DishManager
    {
        private readonly IDishRepository _dishRepository;

        public DishManager(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public async Task<List<Dish>> GetAllDishesAsync()
        {
            return await _dishRepository.GetAllDishesAsync();
        }

        public async Task<Dish?> GetDishByIdAsync(string id)
        {
            return await _dishRepository.GetDishByIdAsync(id);
        }

        public async Task AddDishAsync(Dish dish)
        {
            // Generate a short random string for DishID (length 10)
            dish.Id = Guid.NewGuid().ToString("N").Substring(0, 10);
            // Add any business logic validation here if needed
            await _dishRepository.AddDishAsync(dish);
        }

        public async Task UpdateDishAsync(Dish dish)
        {
            // Add any business logic validation here if needed
            await _dishRepository.UpdateDishAsync(dish);
        }

        public async Task DeleteDishAsync(string id)
        {
            // Add any business logic validation here if needed
            await _dishRepository.DeleteDishAsync(id);
        }
    }
}

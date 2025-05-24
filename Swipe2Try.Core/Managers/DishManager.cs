using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.Core.Managers
{
    public class DishManager
    {
        private readonly IDishRepository _dishRepository;
        private readonly DishValidator _dishValidator;

        public DishManager(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
            _dishValidator = new DishValidator();
        }

        public async Task<List<Dish>> GetAllDishesAsync()
        {
            return await _dishRepository.GetAllDishesAsync();
        }

        public async Task<Dish?> GetDishByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Dish ID cannot be null or empty", nameof(id));

            return await _dishRepository.GetDishByIdAsync(id);
        }        public async Task<(bool Success, List<string> Errors)> AddDishAsync(Dish dish)
        {
            try
            {
                // Validate dish
                var validationResult = _dishValidator.ValidateForCreation(dish);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                // Generate a short random string for DishID (length 10)
                dish.Id = Guid.NewGuid().ToString("N").Substring(0, 10);
                
                await _dishRepository.AddDishAsync(dish);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to add dish: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> AddDishWithMessageAsync(Dish dish)
        {
            var result = await AddDishAsync(dish);
            if (result.Success)
            {
                return (true, "Dish created successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }        public async Task<(bool Success, List<string> Errors)> UpdateDishAsync(Dish dish)
        {
            try
            {
                // Validate dish
                var validationResult = _dishValidator.ValidateForUpdate(dish);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                await _dishRepository.UpdateDishAsync(dish);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to update dish: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> UpdateDishWithMessageAsync(Dish dish)
        {
            var result = await UpdateDishAsync(dish);
            if (result.Success)
            {
                return (true, "Dish updated successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }

        public async Task<(bool Success, List<string> Errors)> DeleteDishAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return (false, new List<string> { "Invalid dish ID" });

                await _dishRepository.DeleteDishAsync(id);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to delete dish: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> DeleteDishWithMessageAsync(string id)
        {
            var result = await DeleteDishAsync(id);
            if (result.Success)
            {            return (true, "Dish deleted successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }
    }
}

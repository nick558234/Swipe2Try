using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.Core.Managers
{
    public class RestaurantManager
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantValidator _restaurantValidator;

        public RestaurantManager(IRestaurantRepository restaurantRepository, IRestaurantValidator restaurantValidator)
        {
            _restaurantRepository =
                restaurantRepository ?? throw new ArgumentNullException(nameof(restaurantRepository));
            _restaurantValidator = restaurantValidator ?? throw new ArgumentNullException(nameof(restaurantValidator));
        }

        public async Task<List<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _restaurantRepository.GetAllRestaurantsAsync();
        }

        public async Task<List<Restaurant>> GetRestaurantsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            return await _restaurantRepository.GetRestaurantsByUserIdAsync(userId);
        }

        public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Restaurant ID must be a positive integer", nameof(id));

            return await _restaurantRepository.GetRestaurantByIdAsync(id);
        }

        public async Task<(bool Success, List<string> Errors)> AddRestaurantAsync(Restaurant restaurant)
        {
            try
            {
                // Validate restaurant
                var validationResult = await _restaurantValidator.ValidateForCreationAsync(restaurant);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                // The database will auto-generate the ID
                await _restaurantRepository.AddRestaurantAsync(restaurant);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to add restaurant: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> AddRestaurantWithMessageAsync(Restaurant restaurant)
        {
            var result = await AddRestaurantAsync(restaurant);
            if (result.Success)
            {
                return (true, "Restaurant created successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }

        public async Task<(bool Success, List<string> Errors)> UpdateRestaurantAsync(Restaurant restaurant)
        {
            try
            {
                // Validate restaurant
                var validationResult = await _restaurantValidator.ValidateForUpdateAsync(restaurant);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                await _restaurantRepository.UpdateRestaurantAsync(restaurant);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to update restaurant: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> UpdateRestaurantWithMessageAsync(Restaurant restaurant)
        {
            var result = await UpdateRestaurantAsync(restaurant);
            if (result.Success)
            {
                return (true, "Restaurant updated successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }

        public async Task<(bool Success, List<string> Errors)> DeleteRestaurantAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return (false, new List<string> { "Invalid restaurant ID" });

                await _restaurantRepository.DeleteRestaurantAsync(id);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to delete restaurant: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> DeleteRestaurantWithMessageAsync(int id)
        {
            var result = await DeleteRestaurantAsync(id);
            if (result.Success)
            {
                return (true, "Restaurant deleted successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }
    }
}
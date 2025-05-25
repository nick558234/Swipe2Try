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
        private readonly RestaurantValidator _restaurantValidator;

        public RestaurantManager(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantValidator = new RestaurantValidator();
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

        public async Task<Restaurant?> GetRestaurantByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Restaurant ID cannot be null or empty", nameof(id));

            return await _restaurantRepository.GetRestaurantByIdAsync(id);
        }

        public async Task<(bool Success, List<string> Errors)> AddRestaurantAsync(Restaurant restaurant)
        {
            try
            {
                // Validate restaurant
                var validationResult = _restaurantValidator.ValidateForCreation(restaurant);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                // Generate a short random string for RestaurantID (length 10)
                restaurant.Id = Guid.NewGuid().ToString("N").Substring(0, 10);
                
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
                var validationResult = _restaurantValidator.ValidateForUpdate(restaurant);
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

        public async Task<(bool Success, List<string> Errors)> DeleteRestaurantAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return (false, new List<string> { "Invalid restaurant ID" });

                await _restaurantRepository.DeleteRestaurantAsync(id);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to delete restaurant: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> DeleteRestaurantWithMessageAsync(string id)
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

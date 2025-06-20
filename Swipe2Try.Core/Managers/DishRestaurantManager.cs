using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;

namespace Swipe2Try.Core.Managers;

public class DishRestaurantManager : IDishRestaurantManager
{
    private readonly IDishRestaurantRepository _dishRestaurantRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public DishRestaurantManager(
        IDishRestaurantRepository dishRestaurantRepository,
        IDishRepository dishRepository,
        IRestaurantRepository restaurantRepository)
    {
        _dishRestaurantRepository = dishRestaurantRepository;
        _dishRepository = dishRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<List<Dish>> GetAllDishesWithRestaurantsAsync()
    {
        try
        {
            var dishes = await _dishRepository.GetAllDishesAsync();
            
            // For each dish, get its associated restaurants
            foreach (var dish in dishes)
            {
                dish.Restaurants = await _dishRestaurantRepository.GetRestaurantsByDishIdAsync(dish.Id);
                
                // Update the Restaurant property with the first restaurant name
                if (dish.Restaurants.Any())
                {
                    dish.Restaurant = dish.Restaurants.First().Name;
                }
                else
                {
                    dish.Restaurant = "Not Available";
                }
            }

            return dishes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllDishesWithRestaurantsAsync: {ex.Message}");
            return new List<Dish>();
        }
    }

    public async Task<Dish?> GetDishWithRestaurantsAsync(int dishId)
    {
        try
        {
            return await _dishRestaurantRepository.GetDishWithRestaurantsAsync(dishId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDishWithRestaurantsAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Restaurant>> GetAllRestaurantsWithDishesAsync()
    {
        try
        {
            var restaurants = await _restaurantRepository.GetAllRestaurantsAsync();
            
            // For each restaurant, get its associated dishes
            foreach (var restaurant in restaurants)
            {
                restaurant.Dishes = await _dishRestaurantRepository.GetDishesByRestaurantIdAsync(restaurant.Id);
            }

            return restaurants;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllRestaurantsWithDishesAsync: {ex.Message}");
            return new List<Restaurant>();
        }
    }

    public async Task<Restaurant?> GetRestaurantWithDishesAsync(int restaurantId)
    {
        try
        {
            return await _dishRestaurantRepository.GetRestaurantWithDishesAsync(restaurantId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRestaurantWithDishesAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> LinkDishToRestaurantAsync(int dishId, int restaurantId)
    {
        try
        {
            // Validate that both dish and restaurant exist
            var dish = await _dishRepository.GetDishByIdAsync(dishId);
            var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);

            if (dish == null || restaurant == null)
            {
                return false;
            }

            // Optionally validate ownership (dish and restaurant should belong to same user)
            if (dish.UserId != restaurant.UserId)
            {
                Console.WriteLine($"Warning: Linking dish {dishId} to restaurant {restaurantId} with different owners");
            }

            return await _dishRestaurantRepository.LinkDishToRestaurantAsync(dishId, restaurantId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LinkDishToRestaurantAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UnlinkDishFromRestaurantAsync(int dishId, int restaurantId)
    {
        try
        {
            return await _dishRestaurantRepository.UnlinkDishFromRestaurantAsync(dishId, restaurantId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UnlinkDishFromRestaurantAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Restaurant>> GetRestaurantsByUserIdAsync(string userId)
    {
        try
        {
            return await _restaurantRepository.GetRestaurantsByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetRestaurantsByUserIdAsync: {ex.Message}");
            return new List<Restaurant>();
        }
    }

    public async Task<List<Dish>> GetDishesByUserIdAsync(string userId)
    {
        try
        {
            return await _dishRepository.GetDishesByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetDishesByUserIdAsync: {ex.Message}");
            return new List<Dish>();
        }
    }

    public async Task<(bool Success, int LinksCreated, string Message)> LinkDishesByOwnershipAsync()
    {
        try
        {
            int linksCreated = await _dishRestaurantRepository.LinkDishesByOwnershipAsync();
            
            if (linksCreated > 0)
            {
                return (true, linksCreated, $"Successfully linked {linksCreated} dish-restaurant relationships based on ownership.");
            }
            else
            {
                return (true, 0, "No new dish-restaurant relationships needed to be created. All existing dishes are already linked to their owner's restaurants.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LinkDishesByOwnershipAsync: {ex.Message}");
            return (false, 0, $"Error linking dishes by ownership: {ex.Message}");
        }
    }

    public async Task<List<Restaurant>> GetAvailableRestaurantsForDishAsync(int dishId)
    {
        try
        {
            var dish = await _dishRepository.GetDishByIdAsync(dishId);
            if (dish == null)
            {
                return new List<Restaurant>();
            }

            // Get all restaurants owned by the same user as the dish
            var userRestaurants = await _restaurantRepository.GetRestaurantsByUserIdAsync(dish.UserId);
            
            return userRestaurants;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAvailableRestaurantsForDishAsync: {ex.Message}");
            return new List<Restaurant>();
        }
    }

    public async Task<bool> IsDishLinkedToRestaurantAsync(int dishId, int restaurantId)
    {
        try
        {
            return await _dishRestaurantRepository.IsDishLinkedToRestaurantAsync(dishId, restaurantId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in IsDishLinkedToRestaurantAsync: {ex.Message}");
            return false;
        }
    }
}

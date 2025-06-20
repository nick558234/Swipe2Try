using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Interfaces;

public interface IDishRestaurantManager
{
    /// <summary>
    /// Get all dishes with their associated restaurants
    /// </summary>
    Task<List<Dish>> GetAllDishesWithRestaurantsAsync();

    /// <summary>
    /// Get a specific dish with its associated restaurants
    /// </summary>
    Task<Dish?> GetDishWithRestaurantsAsync(int dishId);

    /// <summary>
    /// Get all restaurants with their associated dishes
    /// </summary>
    Task<List<Restaurant>> GetAllRestaurantsWithDishesAsync();

    /// <summary>
    /// Get a specific restaurant with its associated dishes
    /// </summary>
    Task<Restaurant?> GetRestaurantWithDishesAsync(int restaurantId);

    /// <summary>
    /// Link a dish to a restaurant
    /// </summary>
    Task<bool> LinkDishToRestaurantAsync(int dishId, int restaurantId);

    /// <summary>
    /// Unlink a dish from a restaurant
    /// </summary>
    Task<bool> UnlinkDishFromRestaurantAsync(int dishId, int restaurantId);

    /// <summary>
    /// Get restaurants owned by a specific user
    /// </summary>
    Task<List<Restaurant>> GetRestaurantsByUserIdAsync(string userId);

    /// <summary>
    /// Get dishes owned by a specific user
    /// </summary>
    Task<List<Dish>> GetDishesByUserIdAsync(string userId);

    /// <summary>
    /// Link all dishes to restaurants based on ownership (same UserId)
    /// </summary>
    Task<(bool Success, int LinksCreated, string Message)> LinkDishesByOwnershipAsync();

    /// <summary>
    /// Get all available restaurants for linking to a dish (owned by the same user)
    /// </summary>
    Task<List<Restaurant>> GetAvailableRestaurantsForDishAsync(int dishId);

    /// <summary>
    /// Check if a dish is linked to a restaurant
    /// </summary>
    Task<bool> IsDishLinkedToRestaurantAsync(int dishId, int restaurantId);
}

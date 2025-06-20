using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Interfaces;

public interface IDishRestaurantRepository
{
    /// <summary>
    /// Get all dish-restaurant relationships
    /// </summary>
    Task<List<DishRestaurant>> GetAllDishRestaurantsAsync();

    /// <summary>
    /// Get dish-restaurant relationships for a specific dish
    /// </summary>
    Task<List<DishRestaurant>> GetDishRestaurantsByDishIdAsync(int dishId);

    /// <summary>
    /// Get dish-restaurant relationships for a specific restaurant
    /// </summary>
    Task<List<DishRestaurant>> GetDishRestaurantsByRestaurantIdAsync(int restaurantId);

    /// <summary>
    /// Get all restaurants associated with a specific dish
    /// </summary>
    Task<List<Restaurant>> GetRestaurantsByDishIdAsync(int dishId);

    /// <summary>
    /// Get all dishes associated with a specific restaurant
    /// </summary>
    Task<List<Dish>> GetDishesByRestaurantIdAsync(int restaurantId);

    /// <summary>
    /// Link a dish to a restaurant
    /// </summary>
    Task<bool> LinkDishToRestaurantAsync(int dishId, int restaurantId);

    /// <summary>
    /// Unlink a dish from a restaurant
    /// </summary>
    Task<bool> UnlinkDishFromRestaurantAsync(int dishId, int restaurantId);

    /// <summary>
    /// Check if a dish is linked to a restaurant
    /// </summary>
    Task<bool> IsDishLinkedToRestaurantAsync(int dishId, int restaurantId);

    /// <summary>
    /// Link dishes to restaurants based on ownership (same UserId)
    /// </summary>
    Task<int> LinkDishesByOwnershipAsync();

    /// <summary>
    /// Get dish with its associated restaurants
    /// </summary>
    Task<Dish?> GetDishWithRestaurantsAsync(int dishId);

    /// <summary>
    /// Get restaurant with its associated dishes
    /// </summary>
    Task<Restaurant?> GetRestaurantWithDishesAsync(int restaurantId);
}

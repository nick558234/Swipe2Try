using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Interfaces;

public interface IRestaurantCategoryRepository
{
    /// <summary>
    /// Get all restaurant-category relationships
    /// </summary>
    Task<List<RestaurantCategory>> GetAllRestaurantCategoriesAsync();

    /// <summary>
    /// Get categories for a specific restaurant
    /// </summary>
    Task<List<Category>> GetCategoriesByRestaurantIdAsync(int restaurantId);

    /// <summary>
    /// Get restaurants for a specific category
    /// </summary>
    Task<List<Restaurant>> GetRestaurantsByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Link a restaurant to a category
    /// </summary>
    Task<bool> LinkRestaurantToCategoryAsync(int restaurantId, int categoryId);

    /// <summary>
    /// Unlink a restaurant from a category
    /// </summary>
    Task<bool> UnlinkRestaurantFromCategoryAsync(int restaurantId, int categoryId);

    /// <summary>
    /// Check if a restaurant is linked to a category
    /// </summary>
    Task<bool> IsRestaurantLinkedToCategoryAsync(int restaurantId, int categoryId);

    /// <summary>
    /// Get restaurant with its categories
    /// </summary>
    Task<Restaurant?> GetRestaurantWithCategoriesAsync(int restaurantId);

    /// <summary>
    /// Update all categories for a restaurant (remove existing and add new ones)
    /// </summary>
    Task<bool> UpdateRestaurantCategoriesAsync(int restaurantId, List<int> categoryIds);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Interfaces;

public interface IDishCategoryRepository
{
    /// <summary>
    /// Get all dish-category relationships
    /// </summary>
    Task<List<DishCategory>> GetAllDishCategoriesAsync();

    /// <summary>
    /// Get categories for a specific dish
    /// </summary>
    Task<List<Category>> GetCategoriesByDishIdAsync(int dishId);

    /// <summary>
    /// Get dishes for a specific category
    /// </summary>
    Task<List<Dish>> GetDishesByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Link a dish to a category
    /// </summary>
    Task<bool> LinkDishToCategoryAsync(int dishId, int categoryId);

    /// <summary>
    /// Unlink a dish from a category
    /// </summary>
    Task<bool> UnlinkDishFromCategoryAsync(int dishId, int categoryId);

    /// <summary>
    /// Check if a dish is linked to a category
    /// </summary>
    Task<bool> IsDishLinkedToCategoryAsync(int dishId, int categoryId);

    /// <summary>
    /// Get dish with its categories
    /// </summary>
    Task<Dish?> GetDishWithCategoriesAsync(int dishId);

    /// <summary>
    /// Update all categories for a dish (remove existing and add new ones)
    /// </summary>
    Task<bool> UpdateDishCategoriesAsync(int dishId, List<int> categoryIds);
}

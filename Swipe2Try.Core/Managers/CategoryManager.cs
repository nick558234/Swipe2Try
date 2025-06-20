using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.Core.Managers;

public class CategoryManager
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDishCategoryRepository _dishCategoryRepository;
    private readonly IRestaurantCategoryRepository _restaurantCategoryRepository;
    private readonly ICategoryValidator _categoryValidator;

    public CategoryManager(
        ICategoryRepository categoryRepository, 
        IDishCategoryRepository dishCategoryRepository,
        IRestaurantCategoryRepository restaurantCategoryRepository,
        ICategoryValidator categoryValidator)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _dishCategoryRepository = dishCategoryRepository ?? throw new ArgumentNullException(nameof(dishCategoryRepository));
        _restaurantCategoryRepository = restaurantCategoryRepository ?? throw new ArgumentNullException(nameof(restaurantCategoryRepository));
        _categoryValidator = categoryValidator ?? throw new ArgumentNullException(nameof(categoryValidator));
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllCategoriesAsync() as List<Category> ?? new List<Category>();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id) // Changed string to int
    {
        if (id <= 0) // Changed validation for int
            throw new ArgumentException("Category ID must be a positive integer", nameof(id));

        return await _categoryRepository.GetCategoryByIdAsync(id);
    }

    public async Task<(bool Success, List<string> Errors)> AddCategoryAsync(Category category)
    {
        try
        {
            // Validate category
            var validationResult = await _categoryValidator.ValidateForCreationAsync(category);
            if (!validationResult.IsValid)
                return (false, validationResult.Errors);

            // category.Id will be set by the database

            await _categoryRepository.AddCategoryAsync(category);
            return (true, new List<string>());
        }
        catch (Exception ex)
        {
            return (false, new List<string> { $"Failed to add category: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string Message)> AddCategoryWithMessageAsync(Category category)
    {
        var result = await AddCategoryAsync(category);
        if (result.Success)
            return (true, "Category created successfully!");
        else
            return (false, string.Join(", ", result.Errors));
    }

    public async Task<(bool Success, List<string> Errors)> UpdateCategoryAsync(Category category)
    {
        try
        {
            // Validate category
            var validationResult = await _categoryValidator.ValidateForUpdateAsync(category);
            if (!validationResult.IsValid)
                return (false, validationResult.Errors);

            await _categoryRepository.UpdateCategoryAsync(category);
            return (true, new List<string>());
        }
        catch (Exception ex)
        {
            return (false, new List<string> { $"Failed to update category: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string Message)> UpdateCategoryWithMessageAsync(Category category)
    {
        var result = await UpdateCategoryAsync(category);
        if (result.Success)
            return (true, "Category updated successfully!");
        else
            return (false, string.Join(", ", result.Errors));
    }

    public async Task<(bool Success, List<string> Errors)> DeleteCategoryAsync(int id) // Changed string to int
    {
        try
        {
            if (id <= 0) // Changed validation for int
                return (false, new List<string> { "Invalid category ID" });

            await _categoryRepository.DeleteCategoryAsync(id);
            return (true, new List<string>());
        }
        catch (Exception ex)
        {
            return (false, new List<string> { $"Failed to delete category: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string Message)>
        DeleteCategoryWithMessageAsync(int id) // Changed string to int
    {
        var result = await DeleteCategoryAsync(id);
        if (result.Success)
            return (true, "Category deleted successfully!");
        else
            return (false, string.Join(", ", result.Errors));
    }

    // Dish-Category relationship methods
    public async Task<List<Category>> GetCategoriesForDishAsync(int dishId)
    {
        if (dishId <= 0)
            throw new ArgumentException("Dish ID must be a positive integer", nameof(dishId));

        var dishCategories = await _dishCategoryRepository.GetCategoriesByDishIdAsync(dishId);
        return dishCategories?.ToList() ?? new List<Category>();
    }

    public async Task<List<Dish>> GetDishesByCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("Category ID must be a positive integer", nameof(categoryId));

        var dishes = await _dishCategoryRepository.GetDishesByCategoryIdAsync(categoryId);
        return dishes?.ToList() ?? new List<Dish>();
    }    public async Task<(bool Success, string Message)> AssignCategoriesToDishAsync(int dishId, List<int> categoryIds)
    {
        try
        {
            if (dishId <= 0)
                return (false, "Invalid dish ID");

            if (categoryIds == null || categoryIds.Count == 0)
                return (false, "No categories selected");

            // Use the existing UpdateDishCategoriesAsync method
            var success = await _dishCategoryRepository.UpdateDishCategoriesAsync(dishId, categoryIds);
            
            return success 
                ? (true, "Categories assigned successfully!") 
                : (false, "Failed to assign categories");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to assign categories: {ex.Message}");
        }
    }    public async Task<(bool Success, string Message)> RemoveCategoryFromDishAsync(int dishId, int categoryId)
    {
        try
        {
            if (dishId <= 0 || categoryId <= 0)
                return (false, "Invalid dish or category ID");

            var success = await _dishCategoryRepository.UnlinkDishFromCategoryAsync(dishId, categoryId);
            return success 
                ? (true, "Category removed successfully!") 
                : (false, "Failed to remove category");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to remove category: {ex.Message}");
        }
    }

    // Restaurant-Category relationship methods
    public async Task<List<Category>> GetCategoriesForRestaurantAsync(int restaurantId)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be a positive integer", nameof(restaurantId));

        var restaurantCategories = await _restaurantCategoryRepository.GetCategoriesByRestaurantIdAsync(restaurantId);
        return restaurantCategories?.ToList() ?? new List<Category>();
    }

    public async Task<List<Restaurant>> GetRestaurantsByCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
            throw new ArgumentException("Category ID must be a positive integer", nameof(categoryId));

        var restaurants = await _restaurantCategoryRepository.GetRestaurantsByCategoryIdAsync(categoryId);
        return restaurants?.ToList() ?? new List<Restaurant>();
    }    public async Task<(bool Success, string Message)> AssignCategoriesToRestaurantAsync(int restaurantId, List<int> categoryIds)
    {
        try
        {
            if (restaurantId <= 0)
                return (false, "Invalid restaurant ID");

            if (categoryIds == null || categoryIds.Count == 0)
                return (false, "No categories selected");

            // Use the existing UpdateRestaurantCategoriesAsync method
            var success = await _restaurantCategoryRepository.UpdateRestaurantCategoriesAsync(restaurantId, categoryIds);
            
            return success 
                ? (true, "Categories assigned successfully!") 
                : (false, "Failed to assign categories");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to assign categories: {ex.Message}");
        }
    }    public async Task<(bool Success, string Message)> RemoveCategoryFromRestaurantAsync(int restaurantId, int categoryId)
    {
        try
        {
            if (restaurantId <= 0 || categoryId <= 0)
                return (false, "Invalid restaurant or category ID");

            var success = await _restaurantCategoryRepository.UnlinkRestaurantFromCategoryAsync(restaurantId, categoryId);
            return success 
                ? (true, "Category removed successfully!") 
                : (false, "Failed to remove category");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to remove category: {ex.Message}");
        }
    }
}
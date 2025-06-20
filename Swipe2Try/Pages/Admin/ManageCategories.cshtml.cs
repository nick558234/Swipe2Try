using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Helpers;

namespace Swipe2Try.Pages.Admin;

public class ManageCategoriesModel : PageModel
{
    private readonly CategoryManager _categoryManager;
    private readonly DishManager _dishManager;
    private readonly RestaurantManager _restaurantManager;

    public ManageCategoriesModel(
        CategoryManager categoryManager,
        DishManager dishManager,
        RestaurantManager restaurantManager)
    {
        _categoryManager = categoryManager;
        _dishManager = dishManager;
        _restaurantManager = restaurantManager;
    }

    public List<Category> Categories { get; set; } = new();
    public List<Dish> Dishes { get; set; } = new();
    public List<Restaurant> Restaurants { get; set; } = new();
    public List<Dish> DishesWithCategories { get; set; } = new();
    public List<Restaurant> RestaurantsWithCategories { get; set; } = new();
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    [BindProperty]
    public int SelectedDishId { get; set; }

    [BindProperty]
    public int SelectedRestaurantId { get; set; }

    [BindProperty]
    public List<int> SelectedCategoryIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAssignDishCategoriesAsync()
    {
        try
        {
            if (SelectedDishId <= 0)
            {
                Message = "Please select a dish.";
                IsSuccess = false;
            }
            else
            {
                var result = await _categoryManager.AssignCategoriesToDishAsync(SelectedDishId, SelectedCategoryIds);
                Message = result.Message;
                IsSuccess = result.Success;
            }
        }
        catch (Exception ex)
        {
            Message = $"Error: {ex.Message}";
            IsSuccess = false;
        }

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAssignRestaurantCategoriesAsync()
    {
        try
        {
            if (SelectedRestaurantId <= 0)
            {
                Message = "Please select a restaurant.";
                IsSuccess = false;
            }
            else
            {
                var result = await _categoryManager.AssignCategoriesToRestaurantAsync(SelectedRestaurantId, SelectedCategoryIds);
                Message = result.Message;
                IsSuccess = result.Success;
            }
        }
        catch (Exception ex)
        {
            Message = $"Error: {ex.Message}";
            IsSuccess = false;
        }

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnGetDishCategoriesAsync(int dishId)
    {
        try
        {
            var categories = await _categoryManager.GetCategoriesForDishAsync(dishId);
            return new JsonResult(categories.Select(c => c.Id));
        }
        catch (Exception)
        {
            return new JsonResult(new List<int>());
        }
    }

    public async Task<IActionResult> OnGetRestaurantCategoriesAsync(int restaurantId)
    {
        try
        {
            var categories = await _categoryManager.GetCategoriesForRestaurantAsync(restaurantId);
            return new JsonResult(categories.Select(c => c.Id));
        }
        catch (Exception)
        {
            return new JsonResult(new List<int>());
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Categories = await _categoryManager.GetAllCategoriesAsync();
            
            // Load all dishes and restaurants with categories
            var allDishes = await _dishManager.GetAllDishesWithCategoriesAsync();
            var allRestaurants = await _restaurantManager.GetAllRestaurantsWithCategoriesAsync();

            // Take a limited number for display
            DishesWithCategories = allDishes.Take(10).ToList();
            RestaurantsWithCategories = allRestaurants.Take(10).ToList();

            // Also keep the full lists for dropdowns
            Dishes = allDishes;
            Restaurants = allRestaurants;
        }
        catch (Exception ex)
        {
            Message = $"Error loading data: {ex.Message}";
            IsSuccess = false;
        }
    }
}

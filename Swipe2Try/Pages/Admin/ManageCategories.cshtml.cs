using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Helpers;

namespace Swipe2Try.Pages.Admin;

public class ManageCategoriesModel : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDishCategoryRepository _dishCategoryRepository;
    private readonly IRestaurantCategoryRepository _restaurantCategoryRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public ManageCategoriesModel(
        ICategoryRepository categoryRepository,
        IDishCategoryRepository dishCategoryRepository,
        IRestaurantCategoryRepository restaurantCategoryRepository,
        IDishRepository dishRepository,
        IRestaurantRepository restaurantRepository)
    {
        _categoryRepository = categoryRepository;
        _dishCategoryRepository = dishCategoryRepository;
        _restaurantCategoryRepository = restaurantCategoryRepository;
        _dishRepository = dishRepository;
        _restaurantRepository = restaurantRepository;
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
                var success = await _dishCategoryRepository.UpdateDishCategoriesAsync(SelectedDishId, SelectedCategoryIds);
                if (success)
                {
                    Message = $"Successfully updated categories for dish.";
                    IsSuccess = true;
                }
                else
                {
                    Message = "Failed to update dish categories.";
                    IsSuccess = false;
                }
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
                var success = await _restaurantCategoryRepository.UpdateRestaurantCategoriesAsync(SelectedRestaurantId, SelectedCategoryIds);
                if (success)
                {
                    Message = $"Successfully updated categories for restaurant.";
                    IsSuccess = true;
                }
                else
                {
                    Message = "Failed to update restaurant categories.";
                    IsSuccess = false;
                }
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
            var categories = await _dishCategoryRepository.GetCategoriesByDishIdAsync(dishId);
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
            var categories = await _restaurantCategoryRepository.GetCategoriesByRestaurantIdAsync(restaurantId);
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
            Categories = await _categoryRepository.GetAllCategoriesAsync();
            Dishes = await _dishRepository.GetAllDishesAsync();
            Restaurants = await _restaurantRepository.GetAllRestaurantsAsync();

            // Load dishes and restaurants with their categories for display
            DishesWithCategories = new List<Dish>();
            foreach (var dish in Dishes.Take(10)) // Limit for display
            {
                var dishWithCategories = await _dishCategoryRepository.GetDishWithCategoriesAsync(dish.Id);
                if (dishWithCategories != null)
                {
                    DishesWithCategories.Add(dishWithCategories);
                }
            }

            RestaurantsWithCategories = new List<Restaurant>();
            foreach (var restaurant in Restaurants.Take(10)) // Limit for display
            {
                var restaurantWithCategories = await _restaurantCategoryRepository.GetRestaurantWithCategoriesAsync(restaurant.Id);
                if (restaurantWithCategories != null)
                {
                    RestaurantsWithCategories.Add(restaurantWithCategories);
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Error loading data: {ex.Message}";
            IsSuccess = false;
        }
    }
}

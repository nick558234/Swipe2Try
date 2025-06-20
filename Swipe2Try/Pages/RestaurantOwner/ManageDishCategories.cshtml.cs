using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Helpers;

namespace Swipe2Try.Pages.RestaurantOwner;

public class ManageDishCategoriesModel : PageModel
{
    private readonly IDishRepository _dishRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDishCategoryRepository _dishCategoryRepository;

    public ManageDishCategoriesModel(
        IDishRepository dishRepository,
        ICategoryRepository categoryRepository,
        IDishCategoryRepository dishCategoryRepository)
    {
        _dishRepository = dishRepository;
        _categoryRepository = categoryRepository;
        _dishCategoryRepository = dishCategoryRepository;
    }    public List<Dish> UserDishes { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Dish> DishesWithCategories { get; set; } = new();    // Properties for the Razor page compatibility
    public List<Dish> Dishes => DishesWithCategories.Any() ? DishesWithCategories : UserDishes;
    public List<Category> AllCategories => Categories;
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    [BindProperty]
    public int SelectedDishId { get; set; }

    [BindProperty]
    public List<int> SelectedCategoryIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadUserDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
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

        await LoadUserDataAsync();
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

    private async Task LoadUserDataAsync()
    {
        try
        {
            var userId = ClaimsHelper.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }            Categories = await _categoryRepository.GetAllCategoriesAsync();
            UserDishes = await _dishRepository.GetDishesByUserIdAsync(userId);

            // Load dishes with their categories for display
            DishesWithCategories = new List<Dish>();
            foreach (var dish in UserDishes) // Load categories for all dishes
            {
                var dishWithCategories = await _dishCategoryRepository.GetDishWithCategoriesAsync(dish.Id);
                if (dishWithCategories != null)
                {
                    DishesWithCategories.Add(dishWithCategories);
                }
                else
                {
                    // If no categories found, add the dish anyway but with empty categories
                    dish.Categories = new List<Category>();
                    DishesWithCategories.Add(dish);
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

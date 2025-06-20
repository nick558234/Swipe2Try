using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Helpers;

namespace Swipe2Try.Pages.RestaurantOwner;

public class ManageDishCategoriesModel : PageModel
{
    private readonly DishManager _dishManager;
    private readonly CategoryManager _categoryManager;

    public ManageDishCategoriesModel(DishManager dishManager, CategoryManager categoryManager)
    {
        _dishManager = dishManager;
        _categoryManager = categoryManager;
    }

    public List<Dish> UserDishes { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Dish> DishesWithCategories { get; set; } = new();

    // Properties for the Razor page compatibility
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

        await LoadUserDataAsync();
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

    private async Task LoadUserDataAsync()
    {
        try
        {
            var userId = ClaimsHelper.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            Categories = await _categoryManager.GetAllCategoriesAsync();
            DishesWithCategories = await _dishManager.GetDishesByUserIdWithCategoriesAsync(userId);
        }
        catch (Exception ex)
        {
            Message = $"Error loading data: {ex.Message}";
            IsSuccess = false;
        }
    }
}

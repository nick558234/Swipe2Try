using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Security.Claims;

namespace Swipe2Try.Pages.RestaurantOwner;

public class ManageDishRestaurantsModel : PageModel
{
    private readonly IDishRestaurantManager _dishRestaurantManager;

    public ManageDishRestaurantsModel(IDishRestaurantManager dishRestaurantManager)
    {
        _dishRestaurantManager = dishRestaurantManager;
    }

    public List<Dish> Dishes { get; set; } = new();
    public List<Restaurant> UserRestaurants { get; set; } = new();
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            // Get user's dishes with their restaurant relationships
            Dishes = await _dishRestaurantManager.GetDishesByUserIdAsync(userId);
            
            // Load restaurants for each dish
            foreach (var dish in Dishes)
            {
                dish.Restaurants = await _dishRestaurantManager.GetRestaurantsByUserIdAsync(userId);
                // Now get only the linked restaurants for this dish
                var linkedRestaurants = new List<Restaurant>();
                var allRestaurants = await _dishRestaurantManager.GetRestaurantsByUserIdAsync(userId);
                
                foreach (var restaurant in allRestaurants)
                {
                    if (await _dishRestaurantManager.IsDishLinkedToRestaurantAsync(dish.Id, restaurant.Id))
                    {
                        linkedRestaurants.Add(restaurant);
                    }
                }
                dish.Restaurants = linkedRestaurants;
            }

            // Get user's restaurants for the dropdown
            UserRestaurants = await _dishRestaurantManager.GetRestaurantsByUserIdAsync(userId);

            return Page();
        }
        catch (Exception ex)
        {
            Message = $"Error loading data: {ex.Message}";
            IsSuccess = false;
            return Page();
        }
    }    public async Task<IActionResult> OnPostAsync(string action, int dishId, int restaurantId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            switch (action?.ToLower())
            {
                case "link":
                    var linkResult = await _dishRestaurantManager.LinkDishToRestaurantAsync(dishId, restaurantId);
                    if (linkResult)
                    {
                        Message = "Dish successfully linked to restaurant.";
                        IsSuccess = true;
                    }
                    else
                    {
                        Message = "Failed to link dish to restaurant.";
                        IsSuccess = false;
                    }
                    break;

                case "unlink":
                    var unlinkResult = await _dishRestaurantManager.UnlinkDishFromRestaurantAsync(dishId, restaurantId);
                    if (unlinkResult)
                    {
                        Message = "Dish successfully unlinked from restaurant.";
                        IsSuccess = true;
                    }
                    else
                    {
                        Message = "Failed to unlink dish from restaurant.";
                        IsSuccess = false;
                    }
                    break;

                case "linkbyownership":
                    var ownershipResult = await _dishRestaurantManager.LinkDishesByOwnershipAsync();
                    Message = ownershipResult.Message;
                    IsSuccess = ownershipResult.Success;
                    break;

                default:
                    Message = "Invalid action specified.";
                    IsSuccess = false;
                    break;
            }

            // Reload the data
            await OnGetAsync();
            return Page();
        }
        catch (Exception ex)
        {
            Message = $"Error processing request: {ex.Message}";
            IsSuccess = false;
            await OnGetAsync();
            return Page();
        }
    }
}

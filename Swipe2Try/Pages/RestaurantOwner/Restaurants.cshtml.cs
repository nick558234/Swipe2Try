using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Swipe2Try.Pages.RestaurantOwner
{
    [Authorize(Roles = "Restaurant Owner")]
    public class RestaurantsModel : PageModel
    {
        private readonly RestaurantManager _restaurantManager;

        public RestaurantsModel(RestaurantManager restaurantManager)
        {
            _restaurantManager = restaurantManager;
        }

        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

        public async Task OnGetAsync()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                // Only get restaurants for the current user
                Restaurants = await _restaurantManager.GetRestaurantsByUserIdAsync(userId);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            // Get the current user's ID for security check
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToPage();
            }

            // Parse the ID to integer
            if (!int.TryParse(id, out int restaurantId))
            {
                TempData["ErrorMessage"] = "Invalid restaurant ID";
                return RedirectToPage();
            }

            // First check if the restaurant belongs to the user
            var restaurant = await _restaurantManager.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
            {
                TempData["ErrorMessage"] = "Restaurant not found";
                return RedirectToPage();
            }

            if (restaurant.UserId != userId)
            {
                TempData["ErrorMessage"] = "Unauthorized to delete this restaurant";
                return RedirectToPage();
            }

            var result = await _restaurantManager.DeleteRestaurantWithMessageAsync(restaurantId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToPage();
        }
    }
}
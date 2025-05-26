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
    public class UpdateRestaurantModel : PageModel
    {
        private readonly RestaurantManager _restaurantManager;

        public UpdateRestaurantModel(RestaurantManager restaurantManager)
        {
            _restaurantManager = restaurantManager;
        }        [BindProperty]
        public Restaurant Input { get; set; } = new Restaurant { Name = "", Location = "", UserId = "" };

        public List<string> ValidationErrors { get; set; } = new List<string>();        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Parse the ID to integer
            if (!int.TryParse(id, out int restaurantId))
            {
                return NotFound();
            }

            var restaurant = await _restaurantManager.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound();
            }

            // Security check: ensure user can only edit their own restaurants
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || restaurant.UserId != userId)
            {
                return Forbid();
            }

            Input = restaurant;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Security check: ensure user can only update their own restaurants
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found";
                return Page();
            }

            if (Input.UserId != userId)
            {
                TempData["ErrorMessage"] = "Unauthorized to update this restaurant";
                return Page();
            }

            // Use RestaurantManager to handle update logic
            var result = await _restaurantManager.UpdateRestaurantWithMessageAsync(Input);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("/RestaurantOwner/Restaurants");
            }
            else
            {
                // For detailed validation errors, still use the detailed method
                var detailedResult = await _restaurantManager.UpdateRestaurantAsync(Input);
                ValidationErrors = detailedResult.Errors;
                return Page();
            }
        }
    }
}

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
    public class CreateRestaurantModel : PageModel
    {
        private readonly RestaurantManager _restaurantManager;

        public CreateRestaurantModel(RestaurantManager restaurantManager)
        {
            _restaurantManager = restaurantManager;
        }

        [BindProperty]
        public Restaurant Input { get; set; } = new Restaurant { Name = "", Location = "" };

        public List<string> ValidationErrors { get; set; } = new List<string>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found";
                return Page();
            }

            // Assign the user ID to the restaurant
            Input.UserId = userId;

            // Use RestaurantManager to handle creation logic
            var result = await _restaurantManager.AddRestaurantWithMessageAsync(Input);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("/RestaurantOwner/Restaurants");
            }
            else
            {
                // For detailed validation errors, still use the detailed method
                var detailedResult = await _restaurantManager.AddRestaurantAsync(Input);
                ValidationErrors = detailedResult.Errors;
                return Page();
            }
        }
    }
}

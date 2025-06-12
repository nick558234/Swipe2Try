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
    [Authorize(Roles = "Admin, Restaurant Owner")]
    public class CreateModel : PageModel
    {
        private readonly DishManager _dishManager;

        public CreateModel(DishManager dishManager)
        {
            _dishManager = dishManager;
        }

        [BindProperty] public Dish Input { get; set; } = new Dish { Name = "", Description = "" };

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

            // Assign the user ID to the dish
            Input.UserId = userId;

            // Use DishManager to handle creation logic
            var result = await _dishManager.AddDishWithMessageAsync(Input);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("/RestaurantOwner/Index");
            }
            else
            {
                // For detailed validation errors, still use the detailed method
                var detailedResult = await _dishManager.AddDishAsync(Input);
                ValidationErrors = detailedResult.Errors;
                return Page();
            }
        }
    }
}
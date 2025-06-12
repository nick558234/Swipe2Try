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
    public class UpdateModel : PageModel
    {
        private readonly DishManager _dishManager;

        public UpdateModel(DishManager dishManager)
        {
            _dishManager = dishManager;
        }

        [BindProperty] public Dish Input { get; set; } = new Dish { Name = "", Description = "" };

        public List<string> ValidationErrors { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var dish = await _dishManager.GetDishByIdAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            // Security check: ensure user can only edit their own dishes
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || dish.UserId != userId)
            {
                return Forbid();
            }

            Input = dish;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Security check: ensure user can only update their own dishes
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found";
                return Page();
            }

            if (Input.UserId != userId)
            {
                TempData["ErrorMessage"] = "Unauthorized to update this dish";
                return Page();
            }

            // Debug logging
            Console.WriteLine($"Update dish - ID: {Input.Id}, Name: {Input.Name}, UserId: {Input.UserId}");

            // Use DishManager to handle update logic
            var result = await _dishManager.UpdateDishWithMessageAsync(Input);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToPage("/RestaurantOwner/Index");
            }
            else
            {
                // For detailed validation errors, still use the detailed method
                var detailedResult = await _dishManager.UpdateDishAsync(Input);
                ValidationErrors = detailedResult.Errors;
                TempData["ErrorMessage"] = string.Join(", ", detailedResult.Errors);
                return Page();
            }
        }
    }
}
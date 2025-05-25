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
    public class IndexModel : PageModel
    {
        private readonly DishManager _dishManager;

        public IndexModel(DishManager dishManager)
        {
            _dishManager = dishManager;
        }

        public List<Dish> Dishes { get; set; } = new List<Dish>();        public async Task OnGetAsync()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                // Only get dishes for the current user
                Dishes = await _dishManager.GetDishesByUserIdAsync(userId);
            }
        }        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            // Get the current user's ID for security check
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToPage();
            }

            // First check if the dish belongs to the user
            var dish = await _dishManager.GetDishByIdAsync(id);
            if (dish == null)
            {
                TempData["ErrorMessage"] = "Dish not found";
                return RedirectToPage();
            }

            if (dish.UserId != userId)
            {
                TempData["ErrorMessage"] = "Unauthorized to delete this dish";
                return RedirectToPage();
            }

            var result = await _dishManager.DeleteDishWithMessageAsync(id);
            
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
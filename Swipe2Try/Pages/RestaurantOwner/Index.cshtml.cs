using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
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

        public List<Dish> Dishes { get; set; } = new List<Dish>();

        public async Task OnGetAsync()
        {
            Dishes = await _dishManager.GetAllDishesAsync();
        }        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
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
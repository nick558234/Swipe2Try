using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
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

        [BindProperty]
        public Dish Dish { get; set; } = new Dish { Name = "", Description = "" };

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Generate a short random string for DishID (length 10)
            // Dish.Id = Guid.NewGuid().ToString("N").Substring(0, 10); // Logic moved to DishManager

            await _dishManager.AddDishAsync(Dish);
            return RedirectToPage("./Index");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
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
        public Dish Input { get; set; } = new Dish { Name = "", Description = "" };

        public List<string> ValidationErrors { get; set; } = new List<string>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Use DishManager to handle creation logic
            var result = await _dishManager.AddDishAsync(Input);
            
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Dish created successfully!";
                return RedirectToPage("/RestaurantOwner/Index");
            }
            else
            {
                ValidationErrors = result.Errors;
                return Page();
            }
        }
    }
}
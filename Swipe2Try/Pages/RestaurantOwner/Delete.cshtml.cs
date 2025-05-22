using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using System.Threading.Tasks;

namespace Swipe2Try.Pages.RestaurantOwner
{
    [Authorize(Roles = "Restaurant Owner")]
    public class DeleteModel : PageModel
    {
        private readonly DishManager _dishManager;

        public DeleteModel(DishManager dishManager)
        {
            _dishManager = dishManager;
        }

        [BindProperty]
        public Dish Dish { get; set; } = new Dish { Name = "", Description = "" };

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Dish = await _dishManager.GetDishByIdAsync(id);

            if (Dish == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _dishManager.DeleteDishAsync(id);
            return RedirectToPage("./Index");
        }
    }
}

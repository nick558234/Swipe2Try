using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Managers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Swipe2Try.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public DeleteModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [BindProperty]
        public Category Category { get; set; } = new Category { Name = string.Empty };

        public async Task<IActionResult> OnGetAsync(int id) // Changed string to int
        {
            if (id <= 0) // Changed check for int
            {
                return NotFound();
            }

            var category = await _categoryManager.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id) // Changed string to int
        {
            if (id <= 0) // Changed check for int
            {
                return NotFound();
            }

            await _categoryManager.DeleteCategoryAsync(id);

            return RedirectToPage("./Index");
        }
    }
}

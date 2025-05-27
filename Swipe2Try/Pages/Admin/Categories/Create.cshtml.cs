using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Managers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Swipe2Try.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public CreateModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [BindProperty] public Category Category { get; set; } = new Category { Name = string.Empty };

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Category.Photo))
            {
                Category.Photo = null;
            }

            await _categoryManager.AddCategoryAsync(Category);

            return RedirectToPage("./Index");
        }
    }
}
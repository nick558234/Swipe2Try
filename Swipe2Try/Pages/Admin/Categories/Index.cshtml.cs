using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Swipe2Try.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public IndexModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public IList<Category> Categories { get; set; } = new List<Category>();

        public async Task OnGetAsync()
        {
            var result = await _categoryManager.GetAllCategoriesAsync();
            if (result != null)
            {
                Categories = result;
            }
        }
    }
}
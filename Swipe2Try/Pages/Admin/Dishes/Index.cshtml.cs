using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Pages.Admin.Dishes
{
    public class IndexModel : PageModel
    {
        private readonly IDishRepository _dishRepository;

        public IndexModel(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public List<Dish> Dishes { get; set; }

        public async Task OnGetAsync()
        {
            Dishes = await _dishRepository.GetAllDishesAsync();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Pages
{
    [Authorize]
    public class swipeModel : PageModel
    {
        private readonly ILogger<swipeModel> _logger;
        private readonly IDishRepository _dishRepository;

        public List<Dish> Dishes { get; set; } = new List<Dish>();

        public swipeModel(ILogger<swipeModel> logger, IDishRepository dishRepository)
        {
            _logger = logger;
            _dishRepository = dishRepository;
        }

        public async Task OnGetAsync()
        {
            try
            {
                Dishes = await _dishRepository.GetAllDishesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dishes");
                Dishes = new List<Dish>();
            }
        }
    }
}
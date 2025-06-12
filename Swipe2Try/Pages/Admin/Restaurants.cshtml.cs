using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Swipe2Try.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RestaurantsModel : PageModel
    {
        private readonly RestaurantManager _restaurantManager;

        public RestaurantsModel(RestaurantManager restaurantManager)
        {
            _restaurantManager = restaurantManager;
        }

        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

        public async Task OnGetAsync()
        {
            Restaurants = await _restaurantManager.GetAllRestaurantsAsync();
        }
    }
}
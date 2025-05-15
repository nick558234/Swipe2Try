using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Swipe2Try.Pages.Admin.Dishes
{
    [Authorize(Roles = "ADMIN, Restaurant Owner")]	
    public class IndexModel : PageModel
    {
        private readonly IDishRepository _dishRepository;
        private readonly IUserManager _userManager;

        public IndexModel(IDishRepository dishRepository, IUserManager userManager)
        {
            _dishRepository = dishRepository;
            _userManager = userManager;
        }

        public List<Dish> Dishes { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }

        public async Task OnGetAsync()
        {
            // Get logged in user information from claims
            UserName = User.Identity?.Name ?? "Guest";
            UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

            Dishes = await _dishRepository.GetAllDishesAsync();
        }
    }
}
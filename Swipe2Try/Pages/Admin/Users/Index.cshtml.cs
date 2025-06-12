using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Swipe2Try.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public IndexModel(UserManager userManager, RoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<User> Users { get; set; } = new();
        public Dictionary<string, string> RoleNames { get; set; } = new();
        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserRole { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            CurrentUserName = User.Identity?.Name ?? "Guest";
            CurrentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

            Users = await _userManager.GetAllUsersAsync();

            // Get all roles for displaying role names using the RoleManager
            var roles = await _roleManager.GetAllRolesAsync();
            foreach (var role in roles)
            {
                RoleNames[role.RoleID] = role.RoleName;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Swipe2Try.Pages.Admin.Users
{
    [Authorize(Roles = "ADMIN")] // Require Admin role for this page
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public IndexModel(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public List<User> Users { get; set; } = new();
        public Dictionary<string, string> RoleNames { get; set; } = new();
        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserRole { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Use built-in User claims instead of session
            CurrentUserName = User.Identity?.Name ?? "Guest";
            CurrentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

            // Get all users
            Users = await _userRepository.GetAllUsersAsync();

            // Get all roles for displaying role names
            var roles = await _roleRepository.GetAllRolesAsync();
            foreach (var role in roles)
            {
                RoleNames[role.RoleID] = role.RoleName;
            }
        }
    }
}